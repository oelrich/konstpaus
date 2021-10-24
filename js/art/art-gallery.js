(function () {

  const get_and_advance = function (date) {
    const year = date.getFullYear();
    const month = date.getMonth() + 1; // Because the author of Date() is a fucking moron.
    const day = date.getDate();
    const day_date = year + "-" + ("00" + month).slice(-2) + "-" + ("00" + day).slice(-2);
    const day_url = "/arts/" + year + "/" + ("00" + month).slice(-2) + "/" + ("00" + day).slice(-2) + "/";
    date.setTime(date.getTime() + 1000 * 60 * 60 * 24);
    return { day: day_date, url: day_url };
  };

  const get_art_urls = function () {
    const today = new Date();
    const date = new Date(2021, 8, 21); // Because the author of Date() is a fucking moron.
    const urls = [];
    while (date.getTime() <= today.getTime()) {
      urls.push(get_and_advance(date));
    }
    return urls;
  };

  const get_art_documents = async function () {
    const art_json_list = [];
    const when_and_where = get_art_urls();
    await Promise.all(
      when_and_where.map(async ww => {
        await fetch(ww.url + "art.json")
          .then(result => result.json())
          .then(json => {
            json.day = ww.day;
            json.work = ww.url + "art_00.jpg";
            art_json_list.push(json);
          })
          .catch(err => console.log(err));
      }));
    return art_json_list;
  };

  const template = document.createElement("template");
  template.innerHTML = `
  <style>
    .gallery {
      display: flex;
      flex-flow: row wrap;
      justify-content: space-evenly;
      gap: 15px;
    }
  </style>
  <div class="gallery"></div>`;

  class ArtGallery extends HTMLElement {
    constructor() {
      super();
      this.attachShadow({ mode: "open" });
      this.shadowRoot.appendChild(template.content.cloneNode(true));
      this.gallery = this.shadowRoot.querySelector(".gallery");
    }
    static get observedAttributes() {
      return ["loading"];
    }

    // eslint-disable-next-line no-unused-vars
    attributeChangedCallback(_name, _oldValue, _newValue) {
      [...this.gallery.children]
        .sort((a, b) => a.day < b.day)
        .forEach(n => this.gallery.appendChild(n));
    }


    async connectedCallback() {
      const documents = await get_art_documents();
      const gallery = this.shadowRoot.querySelector(".gallery");
      this.loading = true;
      documents.forEach(art => {
        const art_thing = document.createElement("art-thing");
        art_thing.day = art.day;
        art_thing.title = art.title;
        art_thing.abstract = art.abstract;
        art_thing.work = art.work;
        gallery.appendChild(art_thing);
      }
      );
      this.loading = false;
    }

    get loading() {
      return JSON.parse(this.getAttribute("loading"));
    }
    set loading(value) {
      this.setAttribute("loading", JSON.stringify(value));
    }
  }

  window.customElements.define("art-gallery", ArtGallery);
})();