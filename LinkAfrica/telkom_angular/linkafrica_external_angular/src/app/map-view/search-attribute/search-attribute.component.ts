import { Component, OnInit, Input, NgModule } from '@angular/core';
import { LPIService } from '../backendservice';
import { FormsModule } from '@angular/forms';
import { LoaderWaiting } from '../loader';
import { SearchBeans } from './SearchBeans';
import { ErrorMessage } from '../errormessage';
import { MapGeometryAction } from '../mapgeometryaction';

@Component({
  selector: 'app-search-attribute',
  templateUrl: './search-attribute.component.html',
  styleUrls: ['./search-attribute.component.css'],
})
export class SearchAttributeComponent implements OnInit {
  @Input() mmap: any;
  objProvince: any;
  objDistrictMunic: any;
  objLocalMunic: any;
  objAdminRegion: any;
  objFilterType: any;
  SI: any;
  FarmName: string = '';
  TownByName: string = '';
  SGNumber: string = '';
  LPI: string = '';
  Parcel: string = '';
  Portion: string = '';
  othersSearchOutputData: any;
  townsSearchOutputData: any;

  constructor(
    private lpiserver: LPIService,
    private loader: LoaderWaiting,
    private mga: MapGeometryAction,
    private errMsg: ErrorMessage
  ) {
    this.SI = new SearchBeans();
    this.SI.SearchType = 'MN';
    this.townsSearchOutputData = [];
    this.othersSearchOutputData = [];
  }

  ngOnInit(): void {
    this.BindProvince();
    this.objFilterType = 'MN';
  }

  BindProvince() {    
    this.lpiserver.getProvinceData('0').subscribe((Pdata: any) => {
      console.warn(Pdata);
      this.objProvince = Pdata;
      var ddProvince = document.getElementById('ddProvince');
      if (ddProvince != null) {
        ddProvince.innerText = '';
        var option = document.createElement('option');
        option.value = '-1';
        option.innerHTML = 'Select';
        ddProvince.append(option);
      }
      Pdata.forEach(function (VV: any) {
        if (ddProvince != null) {
          var option = document.createElement('option');
          option.value = VV.CODE;
          option.innerHTML = VV.NAME;
          ddProvince.append(option);
        }
      });
    });
  }

  OnChangeType(par: any) {
    this.SI.SearchType = par;
    this.objFilterType = par;
  }

  OnChangeSearchCriteria(e: any) {
    var strSC = e.target.value;
    this.SI.SearchCriteria = strSC;
    var pnlFarmByName = document.getElementById('pnlFarmByName');
    var pnlTownByName = document.getElementById('pnlTownByName');
    var pnlSGNumber = document.getElementById('pnlSGNumber');
    var pnlLPI = document.getElementById('pnlLPI');
    var pnlRPP = document.getElementById('pnlRPP');

    if (pnlFarmByName != null) {
      pnlFarmByName.style.display = 'none';
    }
    if (pnlTownByName != null) {
      pnlTownByName.style.display = 'none';
    }
    if (pnlSGNumber != null) {
      pnlSGNumber.style.display = 'none';
    }
    if (pnlLPI != null) {
      pnlLPI.style.display = 'none';
    }
    if (pnlRPP != null) {
      pnlRPP.style.display = 'none';
    }

    switch (strSC) {
      case 'Farm_Name':
        if (pnlFarmByName != null) {
          pnlFarmByName.style.display = 'block';
        }
        break;
      case 'Town_Name':
        if (pnlTownByName != null) {
          pnlTownByName.style.display = 'block';
        }
        break;
      case 'SG_Number':
        if (pnlSGNumber != null) {
          pnlSGNumber.style.display = 'block';
        }
        break;
      case 'LPI':
        if (pnlLPI != null) {
          pnlLPI.style.display = 'block';
        }
        break;
      case 'RPP':
        if (pnlRPP != null) {
          pnlRPP.style.display = 'block';
        }
        break;
    }
  }

  OnChangeProvince(Prov: any) {
    this.loader.StartLoader();
    this.SI.ProvinceCode = Prov.target.value;
    this.SI.DMCode = '-1';
    this.SI.LMCode = '-1';
    this.SI.ARCode = '-1';

    this.lpiserver
      .getDistrictMunicData(Prov.target.value)
      .subscribe((Ddata: any) => {
        console.warn(Ddata);
        this.objDistrictMunic = Ddata;
        var ddlDM_Metro = document.getElementById('ddlDM_Metro');
        var ddlLM = document.getElementById('ddlLM');
        var ddlRegion = document.getElementById('ddlRegion');
        if (ddlDM_Metro != null) {
          ddlDM_Metro.innerText = '';
          var option = document.createElement('option');
          option.value = '-1';
          option.innerHTML = 'Select';
          ddlDM_Metro.append(option);
        }
        if (ddlLM != null) {
          ddlLM.innerText = '';
          var option = document.createElement('option');
          option.value = '-1';
          option.innerHTML = 'Select';
          ddlLM.append(option);
        }

        if (ddlRegion != null) {
          ddlRegion.innerText = '';
          var option = document.createElement('option');
          option.value = '-1';
          option.innerHTML = 'Select';
          ddlRegion.append(option);
        }
        Ddata.forEach(function (VV: any) {
          if (ddlDM_Metro != null) {
            var option = document.createElement('option');
            option.value = VV.DISTRICT;
            option.innerHTML = VV.DISTRICT_N;
            ddlDM_Metro.append(option);
          }
        });
      });
    this.lpiserver
      .getAdminRegionData(Prov.target.value)
      .subscribe((Ddata: any) => {
        console.warn(Ddata);
        this.objAdminRegion = Ddata;
        var ddlAdminRegion = document.getElementById('ddlAdminRegion');
        if (ddlAdminRegion != null) {
          ddlAdminRegion.innerText = '';
          var option = document.createElement('option');
          option.value = '-1';
          option.innerHTML = 'Select';
          ddlAdminRegion.append(option);
        }
        Ddata.forEach(function (VV: any) {
          if (ddlAdminRegion != null) {
            var option = document.createElement('option');
            option.value = VV.CAPTION;
            option.innerHTML = VV.CAPTION;
            ddlAdminRegion.append(option);
          }
        });
        this.loader.StopLoader();
      });
  }

  OnChangeDM(DM: any) {
    if (this.objFilterType == 'MN') {
      this.loader.StartLoader();
      this.SI.DMCode = DM.target.value;
      this.lpiserver
        .getLocalMunicData(DM.target.value)
        .subscribe((Ddata: any) => {
          console.warn(Ddata);
          this.objLocalMunic = Ddata;
          var ddlLM = document.getElementById('ddlLM');

          if (ddlLM != null) {
            ddlLM.innerText = '';
            var option = document.createElement('option');
            option.value = '-1';
            option.innerHTML = 'Select';
            ddlLM.append(option);
          }
          Ddata.forEach(function (VV: any) {
            if (ddlLM != null) {
              var option = document.createElement('option');
              option.value = VV.BOUNDARYID;
              option.innerHTML = VV.CAPTION;
              ddlLM.append(option);
            }
          });
        });
      this.BindRPPByDMMetro(DM.target.value);
    }
  }

  OnChangeAR(AR: any) {
    if (this.objFilterType == 'AR') {
      this.loader.StartLoader();
      this.SI.ARCode = AR.target.value;
      this.BindRPPByAR(AR.target.value);
    }
  }

  OnChangeRPP(RPPVal: any) {
    this.SI.Region = RPPVal.target.value;
  }

  BindRPPByAR(ArVal: any) {
    this.lpiserver.getRPPDataBYAdminRegion(ArVal).subscribe((Ddata: any) => {
      //console.warn(Ddata);

      this.objLocalMunic = Ddata;
      var ddlRegion = document.getElementById('ddlRegion');
      if (ddlRegion != null) {
        ddlRegion.innerText = '';
        var option = document.createElement('option');
        option.value = '-1';
        option.innerHTML = 'Select';
        ddlRegion.append(option);
      }
      Ddata.forEach(function (VV: any) {
        if (ddlRegion != null) {
          var option = document.createElement('option');
          option.value = VV.MDBCODE;
          option.innerHTML = VV.CAPTIONS;
          ddlRegion.append(option);
        }
      });
      this.loader.StopLoader();
    });
  }

  BindRPPByDMMetro(DMMetroVal: any) {
    this.lpiserver.getRPPDataBYDMMetro(DMMetroVal).subscribe((Ddata: any) => {
      //console.warn(Ddata);
      this.objLocalMunic = Ddata;
      var ddlRegion = document.getElementById('ddlRegion');
      if (ddlRegion != null) {
        ddlRegion.innerText = '';
        var option = document.createElement('option');
        option.value = '-1';
        option.innerHTML = 'Select';
        ddlRegion.append(option);
      }
      Ddata.forEach(function (VV: any) {
        if (ddlRegion != null) {
          var option = document.createElement('option');
          option.value = VV.MDBCODE;
          option.innerHTML = VV.CAPTIONS;
          ddlRegion.append(option);
        }
      });
      this.loader.StopLoader();
    });
  }

  BindRPPByLM(LMVal: any) {
    this.lpiserver.getRPPDataBYLM(LMVal).subscribe((Ddata: any) => {
      console.warn(Ddata);
      this.objLocalMunic = Ddata;
      var ddlRegion = document.getElementById('ddlRegion');
      if (ddlRegion != null) {
        ddlRegion.innerText = '';
        var option = document.createElement('option');
        option.value = '-1';
        option.innerHTML = 'Select';
        ddlRegion.append(option);
      }
      Ddata.forEach(function (VV: any) {
        if (ddlRegion != null) {
          var option = document.createElement('option');
          option.value = VV.MDBCODE;
          option.innerHTML = VV.CAPTIONS;
          ddlRegion.append(option);
        }
      });
      this.loader.StopLoader();
    });
  }

  OnChangeLM(LM: any) {
    if (this.objFilterType == 'MN') {
      this.loader.StartLoader();
      this.SI.LMCode = LM.target.value;
      this.BindRPPByLM(LM.target.value);
    }
  }

  OnSearch() {
    //alert(this.SI.ProvinceCode);

    this.SI.FarmName = this.FarmName;
    this.SI.TownByName = this.TownByName;
    this.SI.SGNumber = this.SGNumber;
    this.SI.LPI = this.LPI;
    this.SI.Parcel = this.Parcel;
    this.SI.Portion = this.Portion;

    //console.log(this.SI);
    this.loader.StartLoader();
    this.lpiserver.getSearchResult(this.SI).subscribe((SearchOpt: any) => {
      var divResult = document.getElementById('divResult');
      var divSearchControl = document.getElementById('divSearchControl');
      if (divResult != null) {
        divResult.style.display = 'none';
      }
      if (divSearchControl != null) {
        divSearchControl.style.display = 'none';
      }

      if (SearchOpt.length > 0) {
        this.BindTable(SearchOpt);
        if (divResult != null) {
          divResult.style.display = 'block';
        }
        this.loader.StopLoader();
      } else {
        this.errMsg.ShowErrorMessage('No Records Found');
        if (divSearchControl != null) {
          divSearchControl.style.display = 'block';
        }
        this.loader.StopLoader();
      }
    });
  }

  //https://ng-bootstrap.github.io/#/components/table/examples

  BindTable(SData: any) {
    let thisLocal = this;
    
    var divSearchResults = document.getElementById('SearchResults');
    var tblTown = document.getElementById('tblTown');
    var tblOther = document.getElementById('tblOther');
    var divRecordCount = document.getElementById('divRecordCount');
    if (divRecordCount != null) {
      divRecordCount.innerHTML = SData.length + " record's retrieved";
    }

    SData.forEach(function (SEData: any) {
      if (thisLocal.SI.SearchCriteria == 'Town_Name') {
        if (divSearchResults != null) {
          if (tblTown != null) {
            tblTown.style.display = 'block';
          }
          if (tblOther != null) {
            tblOther.style.display = 'none';
          }
          thisLocal.BindTown(SData);
        }
      } else {
        if (divSearchResults != null) {
          if (tblTown != null) {
            tblTown.style.display = 'none';
          }
          if (tblOther != null) {
            tblOther.style.display = 'block';
          }
          thisLocal.BindOther(SData);
        }
      }
    });
  }

  BindTown(TData: any) {
    this.townsSearchOutputData = [];
    var localOtherThis = this;

    TData.forEach(function (VV: any) {
      localOtherThis.townsSearchOutputData.push({
        OGR_FID: VV.OGR_FID,
        TAG_VALUE: VV.TAG_VALUE,
        TAG_X: VV.TAG_X,
        TAG_Y: VV.TAG_Y,
        TAG_JUST: VV.TAG_JUST,
        WKT: VV.WKT,
        SG_NUMBER: VV.SG_NUMBER,
        TOWN_CODE: VV.TOWN_CODE,
        REGION: VV.REGION,
        PARCEL: VV.PARCEL,
        PORTION: VV.PORTION,
      });
    });
  }

  BindOther(OData: any) {
    this.othersSearchOutputData = [];
    var localOtherThis = this;
    OData.forEach(function (VV: any) {
      localOtherThis.othersSearchOutputData.push({
        OGR_FID: VV.OGR_FID,
        FARMNAME: VV.FARMNAME,
        LSTATUS: VV.LSTATUS,
        WSTATUS: VV.WSTATUS,
        TAG_VALUE: VV.TAG_VALUE,
        LPI: VV.LPI,
        SG_NUMBER: VV.SG_NUMBER,
        TOWN_CODE: VV.TOWN_CODE,
        REGION: VV.REGION,
        PARCEL: VV.PARCEL,
        PORTION: VV.PORTION,
        WKT: VV.WKT,
      });
    });
  }

  ShowSearchControl() {
    var divResult = document.getElementById('divResult');
    var divSearchControl = document.getElementById('divSearchControl');
    if (divResult != null) {
      divResult.style.display = 'none';
    }
    if (divSearchControl != null) {
      divSearchControl.style.display = 'block';
    }
  }

  ZoomToTownData(Par: any) {
    //alert(Par);
    this.mga.drawSingleWKT(Par);
  }

  ZoomToLPIData(Par: any) {
    //alert(Par)
    this.mga.drawSingleWKT(Par);
  }
}
