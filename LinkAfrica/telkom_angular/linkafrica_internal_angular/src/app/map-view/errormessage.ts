import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ErrorMessage {
  IsErrorStart: any = false;
  IsInfoStart: any = false;
  ErrorDiv: any;
  lblErrorDiv: any;
  InfoDiv: any;
  lblInfoDiv: any;

  ShowErrorMessage(textVal: any) {
    this.ErrorDiv = document.getElementById('divErrorMsg');
    this.lblErrorDiv = document.getElementById('lblErr');
    if (this.IsErrorStart == false) {
      this.ErrorDiv.style.display = 'block';
      this.IsErrorStart = true;
      this.lblErrorDiv.innerHTML = textVal;
      setTimeout(() => {
        this.HideErrorMessage();
      }, 30000);
    }
  }

  HideErrorMessage() {
    this.ErrorDiv = document.getElementById('divErrorMsg');   
    if (this.IsErrorStart == true) {
      //this.ErrorDiv.innerHTML = '';
      this.ErrorDiv.style.display = 'none';
      this.IsErrorStart = false;
    }
  }

  ShowInfoMessage(textVal: any) {
    this.InfoDiv = document.getElementById('divInfoMsg');
    this.lblInfoDiv = document.getElementById('lblInfo');
    if (this.IsInfoStart == false) {
      this.InfoDiv.style.display = 'block';
      this.IsInfoStart = true;
      this.lblInfoDiv.innerHTML = textVal;
      setTimeout(() => {
        this.HideInfoMessage();
      }, 30000);
    }
  }

  HideInfoMessage() {
    this.InfoDiv = document.getElementById('divInfoMsg');
    if (this.IsInfoStart == true) {
      this.InfoDiv.style.display = 'none';
      this.IsInfoStart = false;
    }
  }

  closeDivErrorInfo(par: any) {
    if (par == 'divErrorMsg') {
      this.HideErrorMessage();
    } else if (par == 'divInfoMsg') {
      this.HideInfoMessage();
    }
  }
}
