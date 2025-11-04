import { Component, OnInit, Input, NgModule } from '@angular/core';
import { CaptureDataBeans, IdentifyInputBeans } from '../AppBeans';
import { LPIService } from '../backendservice';
import { LoaderWaiting } from '../loader';

@Component({
  selector: 'app-feature-capture',
  templateUrl: './feature-capture.component.html',
  styleUrls: ['./feature-capture.component.css'],
})
export class FeatureCaptureComponent implements OnInit {
  @Input() mmap: any;
  public CDB: CaptureDataBeans[] = [];
  IIB: any;

  constructor(private lpiserver: LPIService, private loader: LoaderWaiting) {
    this.IIB = new IdentifyInputBeans();
  }

  ngOnInit(): void {}

  CaptureDataFromCoord(_coord: any) {
    var Cor = _coord.split('|');
    //alert(Cor[0] + '---' + Cor[1]);
    var lstVisibleLayer: string[] = [];
    var lyrs = this.mmap.getLayers().getArray();

    for (let i = 0; i < lyrs.length; i++) {
      var lly = lyrs[i];
      var tCls = lly.getClassName();
      var b = tCls.split('___')[0];
      var lName = tCls.split('___')[1];
      if (b == 'O') {
        var chk = document.getElementById(tCls) as HTMLInputElement;
        if (chk != null) {
          if (chk.checked) {
            lstVisibleLayer.push(lName);
          }
        }
      }
    }
    this.IIB = new IdentifyInputBeans();
    this.IIB.XCoord = Cor[0];
    this.IIB.YCoord = Cor[1];
    this.IIB.lstVisibleLayer = lstVisibleLayer;
    var that = this;
    this.lpiserver.getCaptureDataResult(this.IIB).subscribe((data) => {
      //console.warn(data);
      var _dataLayer: any = data;
      _dataLayer.forEach(function (PDataRow: any) {
        PDataRow.forEach(function (CDataRow: any) {
          if (!that.CheckExistingLPI(CDataRow.LPI)) {
            var objCDB = new CaptureDataBeans();
            objCDB.GID = CDataRow.GID;
            objCDB.LPI = CDataRow.LPI;
            objCDB.LAYER = CDataRow.LYR;
            objCDB.WKT = CDataRow.WKT;
            that.CDB.push(objCDB);
          }
        });
      });
      setTimeout(() => {
        this.LoadDataOnDiv();
        this.loader.StopLoader();
      }, 3000);
    });
  }

  LoadDataOnDiv() {
    var ggg = this.CDB;
    //this.CDB = new CaptureDataBeans[]
  }

  DeleteFromClientArray(GID: any) {
    //alert(GID);

    this.CDB.forEach((e) => {
      if (e.GID == GID) {
        this.CDB.pop();
      }
    });
  }

  ZoomToWKT(WKT: any) {
    //alert(WKT);
  }

  CheckExistingLPI(lpi: any) {
    let IsExist: any = false;

    this.CDB.forEach((e) => {
      if (e.LPI == lpi) {
        IsExist = true;
      }
    });

    return IsExist;
  }

  CLearSelectedLPI() {
    while (this.CDB.length) {
      this.CDB.pop();
    }
  }

  SendToParentAppMix() {
    var strLPI = '';
    //document.getElementById('hdnAction')!;.value;
    
    this.CDB.forEach((llpi) => {
      if (strLPI == '') {
        strLPI = llpi.LPI;
      } else {
        strLPI = strLPI + ',' + llpi.LPI;
      }
      strLPI = strLPI + ',' + llpi.LPI;
    });
    var hdnAction = document.getElementById('hdnAction')!;
    var ActionVal = (<HTMLInputElement>(hdnAction)).value;
    switch (ActionVal) {
      case 'RES_CAP':
        this.lpiserver.pushedCaptureParcel(strLPI);
      break;
      case 'LDG_CAP':
        this.lpiserver.pushedCaptureParcelLodgement(strLPI);
      break;
      case 'default':
      break;
    }

   
    alert('Send Successfully');
    this.CLearSelectedLPI();
  }

  SendToParentApp() {
    var strLPI = '';
    var hdnAction = document.getElementById('hdnAction')!;
    var ActionVal = (<HTMLInputElement>(hdnAction)).value;


    this.CDB.forEach((llpi) => {
      switch (ActionVal) {
        case 'RES_CAP':
          this.lpiserver.pushedCaptureParcel(llpi).subscribe(
            (VData: any) => {
              console.log(VData);
            },
            (err: any) => {
              console.log(err);
            }
          );
        break;
        case 'LDG_CAP':
          this.lpiserver.pushedCaptureParcelLodgement(llpi).subscribe(
            (VData: any) => {
              console.log(VData);
            },
            (err: any) => {
              console.log(err);
            }
          );
        break;
        case 'default':
        break;
      }

     
    });
    setTimeout(() => {
      alert('Send Successfully');
      this.CLearSelectedLPI();
    }, 3000);
    
  }
}
