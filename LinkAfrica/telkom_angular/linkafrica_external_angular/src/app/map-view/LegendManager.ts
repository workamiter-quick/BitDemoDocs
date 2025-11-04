import { Injectable } from '@angular/core';
import { LPIService } from './backendservice';

@Injectable({
  providedIn: 'root',
})
export class LegendManager {
  mmap: any;
  GroupName: string[];
  constructor(private lpiserver: LPIService) {
    this.GroupName = [];
  }

  BindLegendManagerControl(_map: any) {
    this.mmap = _map;
    this.lpiserver.getLMData().subscribe((data) => {
      //console.warn(data);
      this.BindControl(data);
    });
  }

  BindControl(par: any) {
    this.GroupName = [];
    var self = this;
    var ctrlLM = document.getElementById('ctrlLegend');
    par.forEach(function (value: any) {
      if (value.GROUP_NAME != null) {
        if (
          self.GroupName.indexOf(value.GROUP_NAME + '|' + value.GROUPID) == -1
        ) {
          self.GroupName.push(value.GROUP_NAME + '|' + value.GROUPID);
        }
      }
    });
    this.GroupName.forEach(function (params: any) {
      var grr = params.split('|');
      let P_div = document.createElement('div');
      let C_div = document.createElement('div');
      C_div.setAttribute(
        'style',
        'background-color: transparent; height: auto; width: 100%; padding-left: 8%;'
      );
      C_div.style.display = 'block';
      let P_img_plus = document.createElement('img');
      P_img_plus.setAttribute('src', 'assets/Icon/Plus.png');
      
      P_img_plus.setAttribute('style', 'width: 12px; cursor: pointer; margin-bottom: 6px; margin-right: 6px;');
      P_img_plus.setAttribute('id', 'imgParentGroup_' + grr[1]);
      P_img_plus.addEventListener('click', (e) => {
        C_div.style.display = C_div.style.display == 'none' ? 'block' : 'none';
      });

      let P_lbl_head = document.createElement('label');
      P_lbl_head.innerHTML = grr[0];
      P_div.appendChild(P_img_plus);
      P_div.appendChild(P_lbl_head);
      ctrlLM?.appendChild(P_div);
      ctrlLM?.appendChild(C_div);
      par.forEach(function (valChild: any) {
        if (valChild.GROUP_NAME == grr[0]) {
          let P_chk_Child = document.createElement('img');
          var imgurl =
            valChild.LAYER_WMS_URL +
            '?REQUEST=GetLegendGraphic&VERSION=1.0.0&FORMAT=image/png&WIDTH=15&HEIGHT=15&LAYER=' +
            valChild.LAYER_NAME +
            '&legend_options=fontName:sans-serif;layout:vertical;forceLabels:on;fontstyle:italic;fontSize:large;';
          P_chk_Child.setAttribute('src', imgurl);
          P_chk_Child.setAttribute('style', 'margin-right: 50%;');

          C_div?.appendChild(P_chk_Child);
        
        }
      });
    });

    par.forEach(function (valChild: any) {
      if (valChild.GROUP_NAME == null) {
        let P_chk_Child = document.createElement('img');
        var imgurl =
          valChild.LAYER_WMS_URL +
          '?REQUEST=GetLegendGraphic&VERSION=1.0.0&FORMAT=image/png&WIDTH=15&HEIGHT=15&LAYER=' +
          valChild.LAYER_NAME +
          '&legend_options=fontName:sans-serif;layout:vertical;forceLabels:on;fontstyle:italic;fontSize:large;';
        P_chk_Child.setAttribute('src', imgurl);
        P_chk_Child.setAttribute('style', 'margin-right: 50%;');
        ctrlLM?.appendChild(P_chk_Child);
      }
    });
  }
}
