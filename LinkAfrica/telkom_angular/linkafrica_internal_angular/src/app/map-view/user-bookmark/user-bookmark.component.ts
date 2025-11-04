import { Component, OnInit, Input, NgModule } from '@angular/core';
import { LPIService } from '../backendservice';
import { ErrorMessage } from '../errormessage';
import { LoaderWaiting } from '../loader';
import { BookmarkBeans } from './BookmarkBeans';
import { MapGeometryAction } from '../mapgeometryaction';

@Component({
  selector: 'app-user-bookmark',
  templateUrl: './user-bookmark.component.html',
  styleUrls: ['./user-bookmark.component.css'],
})
export class UserBookmarkComponent implements OnInit {
  @Input() mmap: any;
  bookmarkData: any;
  bookmarkInput: any;
  BookmarkName: string = '';
  constructor(
    private lpiserver: LPIService,
    private loader: LoaderWaiting,
    private mga: MapGeometryAction,
    private errMsg: ErrorMessage
  ) {
    this.bookmarkInput = new BookmarkBeans;
  }
  ngOnInit(): void {
    this.onLoadPanel();
    setTimeout(() => {
      this.OnLoadBookMarkData();
    }, 300);
  }
  onLoadPanel() {
    this.HideAndShowAddNewBookMark('none');
  }
  OnActiveAddPanel() {
    this.HideAndShowAddNewBookMark('block');
    var txtBookmarkName = document.getElementById('txtBookmarkName');
   this.BookmarkName = '';
  }
  OnSaveNewBookmark() {
    this.bookmarkInput = new BookmarkBeans();
    this.bookmarkInput.Caption = this.BookmarkName;
    this.bookmarkInput.Url = this.mga.GetMapCurrentStatus();
    this.bookmarkInput.UserID = "0";
    this.loader.StartLoader();
    this.lpiserver.saveBookmark(this.bookmarkInput).subscribe((BmOpt: any) => {
      if (BmOpt.length > 0) {
        setTimeout(() => {
          this.OnLoadBookMarkData();
        }, 300);
      }
    });
    this.HideAndShowAddNewBookMark('none');
  }
  OnActiveHidePanel() {
    this.HideAndShowAddNewBookMark('none');   
  }
  HideAndShowAddNewBookMark(par: any) {
    var pnlAddNewBookMark = document.getElementById('pnlAddNewBookMark');
    if (pnlAddNewBookMark != null) {
      pnlAddNewBookMark.style.display = par;
    }
  }
  OnLoadBookMarkData() {
    this.lpiserver.getBookmarkData('0').subscribe((Pdata: any) => {
      console.warn(Pdata);
      this.bookmarkData = [];
      var localOtherThis = this;
      Pdata.forEach(function (VV: any) {
        localOtherThis.bookmarkData.push({
          CAPTION: VV.CAPTION,
          URL: VV.URL,
          BRK_ID: VV.BRK_ID,
        });
      });
      setTimeout(() => {
        this.loader.StopLoader();
      }, 300);
    });
  }
  DeleteBookmark(par: any) {
    this.lpiserver.deleteBookmark(par).subscribe((BmOpt: any) => {
      if (BmOpt.length > 0) {
        setTimeout(() => {
          this.OnLoadBookMarkData();
        }, 300);
      }
    });
  }
  ZoomToBookmark(par: any) {
    //alert(par);
    this.mga.SetMapCurrentStatus(par);
  }
}
