import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class HeaderButton {



  onHeaderBtnclick(par: any) {
    this.closeAllBox();
    var boxName = document.getElementById(par)!;
    if (boxName != null) {
      if (boxName.style.display == '') {
        boxName.style.display = 'block';
      } else if (boxName.style.display == 'block') {
        boxName.style.display = 'none';
      } else if (boxName.style.display == 'none') {
        boxName.style.display = 'block';
      }
    }
  }

  closeAllBox() {
    var boxName = document.getElementById('boxBasemap')!;
    boxName.style.display = 'none';
    boxName = document.getElementById('boxLayerManager')!;
    boxName.style.display = 'none';
    boxName = document.getElementById('boxMapTools')!;
    boxName.style.display = 'none';
    boxName = document.getElementById('boxMapLegend')!;
    boxName.style.display = 'none';
    boxName = document.getElementById('boxMapPrint')!;
    boxName.style.display = 'none';
    boxName = document.getElementById('boxSearch')!;
    boxName.style.display = 'none';
    boxName = document.getElementById('boxSearchByDrawing')!;
    boxName.style.display = 'none';
    boxName = document.getElementById('boxBookmark')!;
    boxName.style.display = 'none';
    boxName = document.getElementById('boxIdentify')!;
    boxName.style.display = 'none';
    boxName = document.getElementById('boxCaptureData')!;
    boxName.style.display = 'none';    
    
  }
}
