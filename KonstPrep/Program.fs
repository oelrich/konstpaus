open Giraffe.ViewEngine

let page page_title description body_class page_attr page_body =
    html [ _lang "en" ] [
        head [] [
            meta [ _name "charset"
                   _content "UTF-8" ]
            title [] [ str page_title ]
            meta [ _name "author"
                   _content "Johan Oelrich" ]
            meta [ _name "description"
                   _content description ]
            meta [ _name "keywords"
                   _content "art pause, konstpaus, konstpauser" ]
            meta [ _name "viewport"
                   _content "width=device-width, initial-scale=1.0" ]
            link [ _href "/css/tailwind.css"
                   _rel "stylesheet"
                   _type "text/css" ]
        ]
        body [ _class
               <| sprintf "bg-yellow-900 text-yellow-300 %s" body_class ] [
            header [ _class "my-5 bg-yellow-300 hover:bg-yellow-100 text-yellow-900 pl-11 py-5" ] [
                a [ _href "/" ] [
                    span [ _class "text-5xl font-black tracking-widest" ] [
                        str "Konstpauser"
                    ]
                ]
            ]
            main page_attr page_body
            footer [ _class "my-5 px-7 py-5 bg-yellow-300 text-yellow-900" ] [
                div [ _class "text-xl" ] [
                    str "2021 - Johan Oelrich"
                ]
            ]
        ]
    ]

let indexView thumbs =
    page
        "Konstpauser"
        "Små saker gjorda under pauser från annat."
        ""
        []
        [ div [ _class "container mx-auto relative" ] [
              h1 [ _class "absolute -left-11" ] [
                  rawText "Dramatic pause &hellip;"
              ]
              div
                  [ _class "pt-11 container grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-5" ]
                  thumbs
          ] ]

let art_page (day: ArtScan.Day) =
    page
        (sprintf "Konstpaus - %s" day.ArtDesc.Title)
        day.ArtDesc.Abstract
        "flex flex-col h-full"
        [ _class "flex-grow" ]
        [ div [ _class "flex flex-row" ] [
              img [ _onclick (sprintf "window.location.href='%s/%s';" day.Path day.Image)
                    _class "cursor-pointer pl-11 w-full h-3/4 rounded-xl object-contain"
                    _src (sprintf "%s/%s" day.Path day.Image)
                    _title day.ArtDesc.Title ]
              div [ _class "container flex flex-col pl-5" ] [
                  span [ _class "pt-5" ] [ str day.Date ]
                  h1 [ _class "pt-5 text-xl font-bold" ] [
                      str day.ArtDesc.Title
                  ]
                  p [ _class "pt-3 pl-3 italic" ] [
                      str day.ArtDesc.Abstract
                  ]
              ]
          ] ]


type Rotate =
    | None
    | Rotate90
    | Rotate180
    | Rotate270

let get_rotation (img: System.Drawing.Image) =
    if Array.contains 0x112 img.PropertyIdList then
        let rot = img.GetPropertyItem 0x112
        let value = System.BitConverter.ToUInt16 rot.Value

        match value with
        | 5us
        | 6us -> System.Drawing.RotateFlipType.Rotate90FlipNone
        | 3us
        | 4us -> System.Drawing.RotateFlipType.Rotate180FlipNone
        | 7us
        | 8us -> System.Drawing.RotateFlipType.Rotate270FlipNone
        | _ -> System.Drawing.RotateFlipType.RotateNoneFlipNone
    else
        System.Drawing.RotateFlipType.RotateNoneFlipNone

let shrinky root path name =
    let image =
        System.Drawing.Image.FromFile(sprintf "%s/%s/%s" root path name)

    let width = 512.0

    let height =
        (width / float image.Width) * float image.Height

    let bim =
        new System.Drawing.Bitmap(image, new System.Drawing.Size(int width, int height))

    bim.RotateFlip <| get_rotation image
    //    let image_writer = new System.IO.File.OpenWrite(sprintf "./%s/thumb-%s" path name)
    bim.Save((sprintf "%s/%s/thumb-%s" root path name), System.Drawing.Imaging.ImageFormat.Jpeg)

let write_art root (day: ArtScan.Day) =
    shrinky root day.Path day.Image

    System.IO.File.WriteAllText(
        (sprintf "%s/%s/index.html" root day.Path),
        (art_page day |> RenderView.AsString.xmlNode)
    )


let create_thumb (day: ArtScan.Day) =
    div [ _onclick (sprintf "window.location.href='%s';" day.Path)
          _class
              "cursor-pointer container rounded-xl bg-yellow-300 text-yellow-900 overflow-hidden static flex flex-row" ] [
        div [ _class "container h-full mx-auto" ] [
            img [ _class "container h-full mx-auto object-cover"
                  _src (sprintf "./%s/thumb-%s" day.Path day.Image)
                  _title day.ArtDesc.Title ]
        ]
        div [ _class "container flex flex-col pl-5" ] [
            span [ _class "pt-5 pr-5 text-right" ] [
                str day.Date
            ]
            span [ _class "pt-5 font-bold" ] [
                str day.ArtDesc.Title
            ]
        ]
    ]



[<EntryPoint>]
let main argv =
    printfn "%A" argv.[0]

    let arts =
        ArtScan.scan (sprintf "%s/arts" argv.[0])

    printfn "found %d entries" (List.length arts)
    List.iter (write_art argv.[0]) arts

    arts
    |> List.map create_thumb
    |> List.rev
    |> indexView
    |> fun index -> System.IO.File.WriteAllText(sprintf "%s/index.html" argv.[0], RenderView.AsString.xmlNode index)

    0
