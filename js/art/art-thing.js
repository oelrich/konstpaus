(function () {
  const template = document.createElement("template");
  template.innerHTML = `
<style>
img {
  width: 350px;
  height: auto;
}
</style>
<div class="art">
  <img class="work" />
  <div class="desc">
    <div class="title"></div>
    <div class="abstract"></div>
    <div class="day"></div>
  </div>
</div>
`;
  class ArtThing extends HTMLElement {
    constructor() {
      super();
      this.attachShadow({ mode: "open" });
      this.shadowRoot.appendChild(template.content.cloneNode(true));
    }

    async connectedCallback() {
      this.shadowRoot.querySelector(".work").src = this.work;
      this.shadowRoot.querySelector(".title").innerHTML = this.title;
      this.shadowRoot.querySelector(".abstract").innerHTML = this.abstract;
      this.shadowRoot.querySelector(".day").innerHTML = this.day;
    }

    get work() {
      return JSON.parse(this.getAttribute("work"));
    }
    set work(value) {
      this.setAttribute("work", JSON.stringify(value));
    }
    get abstract() {
      return JSON.parse(this.getAttribute("abstract"));
    }
    set abstract(value) {
      this.setAttribute("abstract", JSON.stringify(value));
    }
    get title() {
      return JSON.parse(this.getAttribute("title"));
    }
    set title(value) {
      this.setAttribute("title", JSON.stringify(value));
    }
    get day() {
      return JSON.parse(this.getAttribute("day"));
    }
    set day(value) {
      this.setAttribute("day", JSON.stringify(value));
    }
  }

  window.customElements.define("art-thing", ArtThing);
})();