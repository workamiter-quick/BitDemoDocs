import { Injectable } from '@angular/core';
import { LPIService } from './backendservice';
import { LoaderWaiting } from './loader';
import { IdentifyInputBeans } from './AppBeans';

@Injectable({
  providedIn: 'root',
})
export class IdentifyManager {
  mmap: any;
  IIB: any;
  IdentifyResultsData: any;
  constructor(private lpiserver: LPIService, private loader: LoaderWaiting) {
    this.IIB = new IdentifyInputBeans();
  }

  InitIdentify(_map: any) {
    this.mmap = _map;
  }

  StartIdentifyData(_coord: any) {
    this.loader.StartLoader();
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

    this.lpiserver.getIdentifyResult(this.IIB).subscribe((data) => {
      //console.warn(data);
      this.IdentifyResultsData = data;
      this.BindLayerDropdown(data);
      //this.loader.StopLoader();
    });
  }

  BindLayerDropdown(_dataLayer: any) {
    var divInitialMassage = document.getElementById('divInitialMassage');
    var divResults = document.getElementById('divResults');
    if (divInitialMassage != null) {
      divInitialMassage.style.display = 'none';
    }
    if (divResults != null) {
      divResults.style.display = 'block';
    }

    var ddlInfoLayers = document.getElementById('ddlInfoLayers')!;
    if (ddlInfoLayers != null) {
      ddlInfoLayers.innerText = '';
    }    

    _dataLayer.forEach(function (PDataRow: any) {
      PDataRow.forEach(function (CDataRow: any) {
        var option = document.createElement('option')!;
        option.value = CDataRow.LYR;
        option.innerHTML = CDataRow.LYR;
        ddlInfoLayers.append(option);
      });
    });
    this.BindLayerResultAttributes(_dataLayer[0][0].LYR);
    setTimeout(() => {
      this.loader.StopLoader();
    }, 5000);
  }

  ClosedIdentifyBox() {
    var divInitialMassage = document.getElementById('divInitialMassage');
    var divResults = document.getElementById('divResults');
    if (divInitialMassage != null) {
      divInitialMassage.style.display = 'block';
    }
    if (divResults != null) {
      divResults.style.display = 'none';
    }
    var ddlInfoLayers = document.getElementById('ddlInfoLayers');
    if (ddlInfoLayers != null) {
      ddlInfoLayers.innerText = '';
    }
  }
  GetRowAsParLayerName(LyrName: any) {
    var dataRow = this.IdentifyResultsData[0][0];
    this.IdentifyResultsData.forEach(function (PDataRow: any) {
      PDataRow.forEach(function (CDataRow: any) {
        if (CDataRow.LYR == LyrName) {
          dataRow = CDataRow;         
        }
      });
    });
    return dataRow;
  }
  BindLayerResultAttributes(LyrName: any) {
    var divLyrResult = document.getElementById('divLyrResult');
    var datarow = this.GetRowAsParLayerName(LyrName);
    var MLyrName = LyrName.split('(')[0]
        var val = '';
        switch (MLyrName) {          
          case 'managed_backhaul':
            val =
            '<b>gid : </b>' +
            datarow.gid +
            '<br />' +
            '<b>site_name : </b>' +
            datarow.site_name +
            '<br />' +
            '<b>suburb : </b>' +
            datarow.suburb +
            '<br />' +
            '<b>network_type : </b>' +
            datarow.network_type +
            '<br />'
            '<b>status : </b>' +
            datarow.status +
            '<br />';;
            break;
          case 'site_point_layer':
            val =
            '<b>gid : </b>' +
            datarow.gid +
            '<br />' +
            '<b>site_name : </b>' +
            datarow.site_name +
            '<br />' +
            '<b>network_type : </b>' +
            datarow.network_type +
            '<br />' +
            '<b>backhaul_type : </b>' +
            datarow.backhaul_type +
            '<br />'
            '<b>status : </b>' +
            datarow.status +
            '<br />'
            '<b>link_residential : </b>' +
            datarow.link_residential +
            '<br />'
            '<b>link_enterprise : </b>' +
            datarow.link_enterprise +
            '<br />'
            '<b>link_business : </b>' +
            datarow.link_business +
            '<br />'
            '<b>link_backhual : </b>' +
            datarow.link_backhual +
            '<br />'
            '<b>link_connect : </b>' +
            datarow.link_connect +
            '<br />';
            break;
          case 'network_line_layer':
            val =
              '<b>gid : </b>' +
              datarow.gid +
              '<br />' +
              '<b>status : </b>' +
              datarow.status +
              '<br />'
              '<b>cable_type : </b>' +
              datarow.cable_type +
              '<br />'
              '<b>duct : </b>' +
              datarow.duct +
              '<br />'
              '<b>operator : </b>' +
              datarow.operator +
              '<br />';
            break;
          case 'site_polygon_layer':
            val =
              '<b>gid : </b>' +
              datarow.gid +
              '<br />' +
              '<b>site_name : </b>' +
              datarow.site_name +
              '<br />' +
              '<b>status : </b>' +
              datarow.status +
              '<br />'
              '<b>client_name : </b>' +
              datarow.client_name +
              '<br />'
              '<b>network_type : </b>' +
              datarow.network_type +
              '<br />';
            break;
          case 'Housing_Point_Layer':
            val =
              '<b>GID : </b>' +
              datarow.gid +
              '<br />' +
              '<b>Operator : </b>' +
              datarow.operator +
              '<br />' +
              '<b>Status : </b>' +
              datarow.status +
              '<br />' +
              '<b>Master Type : </b>' +
              datarow.master_type +
              '<br />';
            break; 
        }
        if (divLyrResult != null) {
          divLyrResult.innerHTML = val;
        }       
  }
}
