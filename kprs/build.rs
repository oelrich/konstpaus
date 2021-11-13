// build.rs
use std::fs;
use std::fs::DirEntry;
use std::fs::OpenOptions;
use std::io::Write;
use std::path::Path;
struct Year(String);
struct Month(String);
struct Day(String);

fn main() {
    let dest_path = Path::new("../").join("arts.json");
    let mut out_file = OpenOptions::new()
        .write(true)
        .truncate(true)
        .create(true)
        .open(dest_path)
        .unwrap();
    let root = Path::new("../arts");
    let results = arts_dir(root);
    out_file.write_all(results.as_bytes()).unwrap();
}

fn arts_dir(p: &Path) -> String {
    let mut years = Vec::new();
    for de in fs::read_dir(p).unwrap() {
        let de = de.unwrap();
        let year = Year(de.file_name().to_str().unwrap().to_owned());
        years.push(year_dir(de, &year));
    }

    format!("{{\"entries\":[{}]}}", years.join(","))
}
fn year_dir(de: DirEntry, year: &Year) -> String {
    let mut months = Vec::new();
    for de in fs::read_dir(de.path()).unwrap() {
        let de = de.unwrap();
        let month = Month(de.file_name().to_str().unwrap().to_owned());
        months.push(month_dir(de, year, &month));
    }
    format!(
        "{{ \"year\":\"{}\", \"months\": [{}] }}",
        year.0,
        months.join(",")
    )
}
fn month_dir(de: DirEntry, year: &Year, month: &Month) -> String {
    let mut days = Vec::new();
    for de in fs::read_dir(de.path()).unwrap() {
        let de = de.unwrap();
        let day = Day(de.file_name().to_str().unwrap().to_owned());
        days.push(day_dir(de, year, month, &day));
    }
    format!(
        "{{ \"month\":\"{}\", \"days\": [{}] }}",
        month.0,
        days.join(",")
    )
}

fn day_dir(de: DirEntry, year: &Year, month: &Month, day: &Day) -> String {
    let mut arts = Vec::new();
    for de in fs::read_dir(de.path()).unwrap() {
        let de = de.unwrap();
        match de.file_name().to_str().unwrap() {
            "art.json" => arts.push(format!(
                "\"desc\":\"arts/{}/{}/{}/art.json\"",
                year.0, month.0, day.0
            )),
            file => arts.push(format!(
                "\"art\":\"arts/{}/{}/{}/{}\"",
                year.0, month.0, day.0, file
            )),
        }
    }
    format!("{{ \"day\":\"{}\", {} }}", day.0, arts.join(","))
}
