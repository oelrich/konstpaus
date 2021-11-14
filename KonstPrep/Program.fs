open Giraffe.ViewEngine

let indexView thumbs =
    html [] [
        head [] [
            meta [ _name "charset"
                   _content "UTF-8" ]
            title [] [ str "Konstpauser" ]
            meta [ _name "author"
                   _content "Johan Oelrich" ]
            meta [ _name "description"
                   _content "Små saker gjorda under pauser från annat." ]
            meta [ _name "keywords"
                   _content "art pause, konstpaus, konstpauser" ]
            meta [ _name "viewport"
                   _content "width=device-width, initial-scale=1.0" ]
            link [ _href "/css/tailwind.css"
                   _rel "stylesheet"
                   _type "text/css" ]
        ]
        body [] [
            h1 [] [ str "Konstpauser" ]
            div [] thumbs
        ]
    ]

let thumb_view (day: ArtScan.Day) =
    div [] [
        img [ _src (sprintf "%s/thumb-%s" day.Path day.Image)
              _title day.ArtDesc.Title ]
        div [] [ str day.ArtDesc.Title ]
    ]

let art_page (day: ArtScan.Day) =
    html [] [
        head [] [
            meta [ _name "charset"
                   _content "UTF-8" ]
            title [] [
                str (sprintf "Konstpaus - %s" day.ArtDesc.Title)
            ]
            meta [ _name "author"
                   _content "Johan Oelrich" ]
            meta [ _name "description"
                   _content day.ArtDesc.Abstract ]
            meta [ _name "keywords"
                   _content "art pause, konstpaus, konstpauser" ]
            meta [ _name "viewport"
                   _content "width=device-width, initial-scale=1.0" ]
            link [ _href "/css/tailwind.css"
                   _rel "stylesheet"
                   _type "text/css" ]
        ]
        body [] [
            img [ _src (sprintf "%s/%s" day.Path day.Image)
                  _title day.ArtDesc.Title ]
            h1 [] [ str day.ArtDesc.Title ]
            span [] [ str day.Date ]
            p [] [ str day.ArtDesc.Abstract ]
            a [ _href "/" ] [ str "Index" ]

        ]
    ]

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
    bim.Save(System.IO.File.OpenWrite(sprintf "./%s/thumb-%s" path name), System.Drawing.Imaging.ImageFormat.Png)

let write_art (day: ArtScan.Day) =
    shrinky day.Path day.Image
    System.IO.File.WriteAllText((sprintf "./%s/index.html" day.Path), (art_page day |> RenderView.AsString.xmlNode))

let arts = ArtScan.scan "./arts"

arts |> List.iter write_art

let create_thumb (day: ArtScan.Day) =
    a [ _href day.Path ] [
        img [ _src (sprintf "./%s/thumb-%s" day.Path day.Image)
              _title day.ArtDesc.Title ]
        span [] [ str day.Date ]
        span [] [ str day.ArtDesc.Title ]
    ]

arts
|> List.map create_thumb
|> indexView
|> fun index -> System.IO.File.WriteAllText("./index.html", RenderView.AsString.xmlNode index)
