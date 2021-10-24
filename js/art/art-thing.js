(function () {
  const template = document.createElement("template");
  template.innerHTML = `
<style>
.art {
  background-size: cover;
  background-position: center;
  border-radius: 16px;
  width:  350px;
  height: 350px;
}
.work {
  object-fit: cover;
  maxheight: 100%;
  width: 100%;
}
.info {
  width: 100%;
  height: 100%;
  position: relative;
}
.title {
  background-color: rgba(224,224,224,0.85);

  border-radius: 8px;
  padding: 4px;
  text-align: center;

  position: absolute;
  width: 90%;
  left: 50%;
  transform: translate(-50%, -50%);
  top: 95%;
}
.abstract {
  background-color: rgba(224,224,224,0.75);

  border-radius: 8px;
  padding: 4px;
  text-align: center;

  position: absolute;
  width: 90%;
  left: 50%;
  transform: translate(-50%, -50%);
  top: 90%;
}
.day {
  background-color: rgba(224,224,224,0.75);
  
  border-radius: 8px;
  padding: 4px;
  text-align: center;

  position: absolute;
  width: 30%;
  right: 5%;
  top: 5%;
}
</style>
<div class="art">
  <img hidden class="work" />
  <div class="info">
    <div class="title"></div>
    <div hidden class="abstract"></div>
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
      this.shadowRoot.querySelector(".art").setAttribute("style", "background-image: url('" + this.work + "')");
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