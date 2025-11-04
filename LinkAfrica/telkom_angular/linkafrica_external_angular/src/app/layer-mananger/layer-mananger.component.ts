import { Component, OnInit } from '@angular/core';
import { AdminService } from './backendadminservice';

@Component({
  selector: 'app-layer-mananger',
  templateUrl: './layer-mananger.component.html',
  styleUrls: ['./layer-mananger.component.css']
})
export class LayerManangerComponent implements OnInit {
  wmsData: any;
  constructor(private adminserver: AdminService) {}


  ngOnInit(): void {
    this.fnGetAllGroup();
    this.fnGetAllWmsData();
  }
  fnOpenAddGroupWindow() {
    var boxName = document.getElementById('ctrlOverLays')!;
    if (boxName != null) {
      if (boxName.style.display == '') {
        boxName.style.display = 'block';
      } else if (boxName.style.display == 'none') {
        boxName.style.display = 'block';
      } else if (boxName.style.display == 'block') {
        boxName.style.display = 'none';
      }
    }
  }
  fnGetAllWmsData() {
    var localOtherThis = this;
    this.wmsData = [];
    this.adminserver.getAllWmsLayers().subscribe((Pdata: any) => {
      var tblWMS = document.getElementById('tblWMS');
      Pdata.forEach(function (SEData: any) {
        localOtherThis.wmsData.push({
          GROUP_NAME: SEData.GROUP_NAME,
          OBJECTID: SEData.OBJECTID,
          LAYER_ALIAS_NAME: SEData.LAYER_ALIAS_NAME,
          LAYER_WMS_URL: SEData.LAYER_WMS_URL,
          LAYER_NAME: SEData.LAYER_NAME,
          FOMART: SEData.FOMART,
          TRANSPARENT: SEData.TRANSPARENT,
          TILED: SEData.TILED,
          BUFFER: SEData.BUFFER,
          DISPLAY_OUTSIDE_MAX_EXTENT: SEData.DISPLAY_OUTSIDE_MAX_EXTENT,
          BASELAYER: SEData.BASELAYER,
          DISPLAY_IN_LAYERSWITCHER: SEData.DISPLAY_IN_LAYERSWITCHER,
          VISIBILITY: SEData.VISIBILITY,
          ISDELETED: SEData.ISDELETED,
          GROUPID: SEData.GROUPID,
          USERID: SEData.USERID,
          LAYER_INDEX: SEData.LAYER_INDEX,
        });
      });
    });
  }
  fnSaveNewGroup() {
    var txtGroupLayer = (<HTMLInputElement>(
      document.getElementById('txtGroupLayer')
    )).value;

    this.adminserver.saveGroup(txtGroupLayer).subscribe((BmOpt: any) => {
      if (BmOpt.length > 0) {
        setTimeout(() => {
          this.fnGetAllGroup();
        }, 200);

        setTimeout(() => {
          this.fnOpenAddGroupWindow();
        }, 500);
      }
    });
  }
  fnGetAllGroup() {
    this.adminserver.getAllGroupName().subscribe((Pdata: any) => {
      console.warn(Pdata);
      var ddlGroupLayer = document.getElementById('ddlGroupLayer')!;
      if (ddlGroupLayer != null) {
        ddlGroupLayer.innerText = '';
        var option = document.createElement('option');
        option.value = '-1';
        option.innerHTML = 'Select';
        ddlGroupLayer.append(option);
      }
      Pdata.forEach(function (VV: any) {
        if (ddlGroupLayer != null) {
          var option = document.createElement('option');
          option.value = VV.OBJECTID;
          option.innerHTML = VV.GROUPNAME;
          ddlGroupLayer.append(option);
        }
      });
    });
  }

  fnGridActionClick(par: any) {
    var thisLocal = this;
    this.wmsData.forEach(function (SEData: any) {
      if (par == SEData.OBJECTID) {
        (<HTMLInputElement>document.getElementById('txtLayerAliasName')).value =
          SEData.LAYER_ALIAS_NAME;

        (<HTMLInputElement>document.getElementById('txtLayerWMSURL')).value =
          SEData.LAYER_WMS_URL;

        (<HTMLInputElement>document.getElementById('txtLayerName')).value =
          SEData.LAYER_NAME;

        thisLocal.fnSelectDropdown('ddlFormat', SEData.FOMART);
        thisLocal.fnSelectDropdown('ddlTransparent', SEData.TRANSPARENT);
        thisLocal.fnSelectDropdown('ddlTiled', SEData.TILED);

        (<HTMLInputElement>document.getElementById('txtLayerBuffer')).value =
          SEData.BUFFER;

        thisLocal.fnSelectDropdown(
          'ddldisplayOutsideMaxExtent',
          SEData.DISPLAY_OUTSIDE_MAX_EXTENT
        );
        thisLocal.fnSelectDropdown('ddlIsBaseLayer', SEData.BASELAYER);
        thisLocal.fnSelectDropdown(
          'ddldisplayInLayerSwitcher',
          SEData.DISPLAY_IN_LAYERSWITCHER
        );
        thisLocal.fnSelectDropdown('ddlVisibility', SEData.VISIBILITY);
        thisLocal.fnSelectDropdown('ddlGroupLayer', SEData.GROUPID);
        const HdnEditSelectedObjectID = document.getElementById(
          'HdnEditSelectedObjectID'
        )!;
        HdnEditSelectedObjectID.setAttribute('value', SEData.OBJECTID);
      }
    });
  }
  fnSelectDropdown(dropdownName: string, desiredValue: string) {
    const dropdown: HTMLSelectElement | null = document.getElementById(
      dropdownName
    ) as HTMLSelectElement;

    if (dropdown) {
      // Iterate through options to find the desired one
      for (let i = 0; i < dropdown.options.length; i++) {
        if (dropdown.options[i].value === desiredValue) {
          // Set the selectedIndex to the index of the desired option
          dropdown.selectedIndex = i;
          break;
        }
      }
    }
  }

  fnResetUpdate() {
    let ddlDefaultValue: string = 'true';
    (<HTMLInputElement>document.getElementById('txtLayerAliasName')).value = '';

    (<HTMLInputElement>document.getElementById('txtLayerWMSURL')).value = '';

    (<HTMLInputElement>document.getElementById('txtLayerName')).value = '';

    this.fnSelectDropdown('ddlFormat', ddlDefaultValue);
    this.fnSelectDropdown('ddlTransparent', ddlDefaultValue);
    this.fnSelectDropdown('ddlTiled', ddlDefaultValue);

    (<HTMLInputElement>document.getElementById('txtLayerBuffer')).value = '';

    this.fnSelectDropdown('ddldisplayOutsideMaxExtent', ddlDefaultValue);
    this.fnSelectDropdown('ddlIsBaseLayer', ddlDefaultValue);
    this.fnSelectDropdown('ddldisplayInLayerSwitcher', ddlDefaultValue);
    this.fnSelectDropdown('ddlVisibility', ddlDefaultValue);
    this.fnSelectDropdown('ddlGroupLayer', '-1');
    const HdnEditSelectedObjectID = document.getElementById(
      'HdnEditSelectedObjectID'
    )!;
    HdnEditSelectedObjectID.setAttribute('value', '0');
  }

}
