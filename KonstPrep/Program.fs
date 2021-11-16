open Giraffe.ViewEngine

let page page_title description page_body =
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
        body [ _class "bg-purple-900 text-purple-300" ] [
            header [ _class "my-5 bg-purple-300 text-purple-900 pl-11 py-5" ] [
                a [ _href "/" ] [
                    h1 [ _class "text-5xl" ] [
                        str "Konstpauser"
                    ]
                ]
            ]
            div [ _class "container mx-auto" ] page_body
            footer [ _class "my-5 px-7 py-5 bg-purple-300 text-purple-900" ] [
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
        [ div [ _class "container grid grid-cols-3 gap-5" ] thumbs ]

let art_page (day: ArtScan.Day) =
    page
        (sprintf "Konstpaus - %s" day.ArtDesc.Title)
        day.ArtDesc.Abstract
        [ img [ _src (sprintf "%s/%s" day.Path day.Image)
                _title day.ArtDesc.Title ]
          h1 [] [ str day.ArtDesc.Title ]
          span [] [ str day.Date ]
          p [] [ str day.ArtDesc.Abstract ] ]

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

let shrinky path name =
    let image =
        System.Drawing.Image.FromFile(sprintf "./%s/%s" path name)

    let width = 512.0

    let height =
        (width / float image.Width) * float image.Height

    let bim =
        new System.Drawing.Bitmap(image, new System.Drawing.Size(int width, int height))

    bim.RotateFlip <| get_rotation image
    //    let image_writer = new System.IO.File.OpenWrite(sprintf "./%s/thumb-%s" path name)
    bim.Save((sprintf "./%s/thumb-%s" path name), System.Drawing.Imaging.ImageFormat.Jpeg)

let write_art (day: ArtScan.Day) =
    shrinky day.Path day.Image
    System.IO.File.WriteAllText((sprintf "./%s/index.html" day.Path), (art_page day |> RenderView.AsString.xmlNode))

let arts = ArtScan.scan "./arts"

arts |> List.iter write_art

let create_thumb (day: ArtScan.Day) =
    div [ _class "container rounded-xl bg-purple-300 overflow-hidden" ] [
        a [ _class "p-3 container mx-auto"
            _href day.Path ] [
            div [ _class "container mx-auto" ] [
                img [ _class "container mx-auto"
                      _src (sprintf "./%s/thumb-%s" day.Path day.Image)
                      _title day.ArtDesc.Title ]
            ]
            span [] [ str day.Date ]
            span [] [ str day.ArtDesc.Title ]
        ]
    ]

arts
|> List.map create_thumb
|> List.rev
|> indexView
|> fun index -> System.IO.File.WriteAllText("./index.html", RenderView.AsString.xmlNode index)
