import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoaderWaiting {
  IsLoaderStart: any = false;
  LoaderImg: any;
  constructor() {
    
  }
  StartLoader() {      
      this.LoaderImg = document.getElementById('imgLoader');
    if (this.IsLoaderStart == false) {
      this.LoaderImg.style.display = 'block';
      this.IsLoaderStart = true;
      setTimeout(() => {
        this.StopLoader();
      }, 30000);
    }
  }

  StopLoader() {
    this.LoaderImg = document.getElementById('imgLoader');
    if (this.IsLoaderStart == true) {
      this.LoaderImg.style.display = 'none';
      this.IsLoaderStart = false;
    }
  }
}
