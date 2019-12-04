import 'quill';

const CONTAINER_ID = '#counter';

export interface Config {
  container: string;
  maxLength: number;
}

export interface QuillInstance {
  on: any;
  getText: any;
}

export default class Counter {
  constructor(public quill: QuillInstance, public options: Config) {
    // For SSR
    if (!document) { return; }

    const container = document.querySelector(CONTAINER_ID);
    if(!container) { return; }

    this.setHTML(container);

    this.quill.on('text-change', () => {
      this.setHTML(container);
    });
  }

  setHTML(container) {
    const length = this.calculate();
    container.innerHTML = `<span>${length}</span> / ${this.options.maxLength}`;

    if (length > this.options.maxLength) {
      container.classList.add('invalid');
    }
  }

  calculate() {
    const text = this.quill.getText().trim();
    return text.length;
  }
}
