import { Injectable } from '@angular/core';

import { LPIService } from './backendservice';

@Injectable({
  providedIn: 'root',
})
export class LayerManager {
  mmap: any;
  GroupName: string[];
  constructor(private lpiserver: LPIService) {
    this.GroupName = [];
  }
  BindLayerManagerControl(_map: any) {
    this.mmap = _map;
    this.lpiserver.getLMData().subscribe((data) => {
           //console.warn(data);
      this.BindControl(data);
    });
  }
  BindControl(par: any) {
    this.GroupName = [];
    var self = this;
    var ctrlLM = document.getElementById('ctrlLM');
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
      let P_chk_head = document.createElement('input');
      P_chk_head.setAttribute('type', 'checkbox');     
      P_chk_head.setAttribute('id', 'parentGroupChk_' + grr[1]);
      P_chk_head.addEventListener('change', (e) => {
        //alert('Select All checkbox');
        par.forEach(function (valChild: any) {
          if (valChild.GROUP_NAME == grr[0]) {
            let cchk = <HTMLInputElement>(
              document.getElementById(
                'O___' + valChild.LAYER_NAME.split(':')[1]
              )
            );
            cchk.checked = P_chk_head.checked;
            var lyrs = self.mmap.getLayers().getArray();
            for (let i = 0; i < lyrs.length; i++) {
              var lly = lyrs[i];
              var tCls = lly.getClassName();
              var b = tCls.split('___')[0];
              var lName = tCls.split('___')[1];
              if (b == 'O' && lName == valChild.LAYER_NAME.split(':')[1]) {
                lly.setVisible(cchk.checked);
              }
            }
          }
        });
      });

      let P_lbl_head = document.createElement('label');
      P_lbl_head.setAttribute('style', 'cursor: pointer; margin-left: 6px; font-size: large;');
      P_lbl_head.innerHTML = grr[0];

      P_div.appendChild(P_img_plus);
      P_div.appendChild(P_chk_head);
      P_div.appendChild(P_lbl_head);
      ctrlLM?.appendChild(P_div);
      ctrlLM?.appendChild(C_div);
      par.forEach(function (valChild: any) {
        if (valChild.GROUP_NAME == grr[0]) {
          let CC_div = document.createElement('div');
          let P_chk_Child = document.createElement('input');
          P_chk_Child.setAttribute('type', 'checkbox');        
          var res = valChild.VISIBILITY == true ? true : false;
          P_chk_Child.checked = res;
          P_chk_Child.setAttribute(
            'id',
            'O___' + valChild.LAYER_NAME.split(':')[1]
          );
          P_chk_Child.addEventListener('change', (e) => {
            var lyrs = self.mmap.getLayers().getArray();
            for (let i = 0; i < lyrs.length; i++) {
              var lly = lyrs[i];
              var tCls = lly.getClassName();
              var b = tCls.split('___')[0];
              var lName = tCls.split('___')[1];
              if (b == 'O' && lName == valChild.LAYER_NAME.split(':')[1]) {
                lly.setVisible(P_chk_Child.checked);
              }
            }
          });
          let P_lbl_child = document.createElement('label');
          P_lbl_child.setAttribute('style', 'cursor: pointer; margin-left: 6px; font-size: large;');
          P_lbl_child.innerHTML = valChild.LAYER_ALIAS_NAME;
          CC_div?.appendChild(P_chk_Child);
          CC_div?.appendChild(P_lbl_child);
          C_div?.appendChild(CC_div);
        }
      });
    });

    par.forEach(function (valChild: any) {
      if (valChild.GROUP_NAME == null) {
        let CC_div = document.createElement('div');
        let P_chk_Child = document.createElement('input');
        P_chk_Child.setAttribute('type', 'checkbox');
        var res = valChild.VISIBILITY == true ? true : false;
        P_chk_Child.checked = res;
        P_chk_Child.setAttribute(
          'id',
          'O___' + valChild.LAYER_NAME.split(':')[1]
        );
        P_chk_Child.addEventListener('change', (e) => {
          var lyrs = self.mmap.getLayers().getArray();
          for (let i = 0; i < lyrs.length; i++) {
            var lly = lyrs[i];
            var tCls = lly.getClassName();
            var b = tCls.split('___')[0];
            var lName = tCls.split('___')[1];
            if (b == 'O' && lName == valChild.LAYER_NAME.split(':')[1]) {
              lly.setVisible(P_chk_Child.checked);
            }
          }
        });
        let P_lbl_child = document.createElement('label');
        P_lbl_child.setAttribute('style', 'cursor: pointer; margin-left: 6px; font-size: large;');
        P_lbl_child.innerHTML = valChild.LAYER_ALIAS_NAME;
        CC_div?.appendChild(P_chk_Child);
        CC_div?.appendChild(P_lbl_child);

        ctrlLM?.appendChild(CC_div);
      }
    });
  }
}
