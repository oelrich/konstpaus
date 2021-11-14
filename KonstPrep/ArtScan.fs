module ArtScan

open Giraffe.ViewEngine

open System.IO

type ArtDesc = { Title: string; Abstract: string }

let json_options =
    let opt =
        new System.Text.Json.JsonSerializerOptions()

    opt.PropertyNameCaseInsensitive <- true
    opt

let get_art_desc (str: string) =
    System.Text.Json.JsonSerializer.Deserialize<ArtDesc>(str, json_options)

type Day =
    { Year: string
      Month: string
      Day: string
      Date: string
      Path: string
      Image: string
      ArtDesc: ArtDesc }

let read_day year month (dir: DirectoryInfo) =
    { Year = year
      Month = month
      Day = dir.Name
      Date = sprintf "%s-%s-%s" year month dir.Name
      Path = sprintf "/arts/%s/%s/%s" year month dir.Name
      Image =
        dir.EnumerateFiles("*.jpeg")
        |> Seq.head
        |> (fun x -> x.Name)
      ArtDesc =
        dir.EnumerateFiles("*.json")
        |> Seq.head
        |> (fun f -> File.ReadAllText f.FullName |> get_art_desc) }

let read_month year (dir: DirectoryInfo) =
    dir.EnumerateDirectories()
    |> Seq.map (read_day year dir.Name)
    |> List.ofSeq

let read_year (dir: DirectoryInfo) =
    dir.EnumerateDirectories()
    |> Seq.map (read_month dir.Name)
    |> Seq.fold (fun s elt -> List.append s elt) List.empty

let read_arts (dir: DirectoryInfo) =
    dir.EnumerateDirectories()
    |> Seq.map read_year
    |> Seq.fold (fun s elt -> List.append s elt) List.empty

let scan path = DirectoryInfo path |> read_arts
