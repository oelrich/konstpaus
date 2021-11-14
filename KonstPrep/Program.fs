open Giraffe.ViewEngine

let indexView =
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

        ]
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
            h1 [] [ str day.ArtDesc.Title ]
            span [] [ str day.Date ]
            img [ _src (sprintf "%s/%s" day.Path day.Image)
                  _title day.ArtDesc.Title ]
            p [] [ str day.ArtDesc.Abstract ]

        ]
    ]

let write_art (day: ArtScan.Day) =
    System.IO.File.WriteAllText((sprintf "./%s/index.html" day.Path), (art_page day |> RenderView.AsString.xmlNode))

ArtScan.scan "./arts" |> List.iter write_art


// For more information see https://aka.ms/fsharp-console-apps
// printfn "%s" <| RenderView.AsString.xmlNode indexView
