import { Component, OnInit, ViewChild } from '@angular/core';
import Map from 'ol/map';
import View from 'ol/View';
import MousePosition from 'ol/control/MousePosition';
import { createStringXY } from 'ol/coordinate';
import Icon from 'ol/style/Icon';
import BingMaps from 'ol/source/BingMaps';
import OSM from 'ol/source/OSM';
import * as olProj from 'ol/proj';
import * as proj4x from 'proj4';
import { register } from 'ol/proj/proj4';
import TileLayer from 'ol/layer/Tile';
import TileWMS from 'ol/source/TileWMS';
import { ScaleLine, defaults as defaultControls } from 'ol/control';
import 'ol/ol.css';
import WKT from 'ol/format/WKT';
import { LPIService } from './backendservice';
import { HeaderButton } from './header.button';
import { LayerManager } from './LayerManager';
import { IdentifyManager } from './identify-map';
import { LegendManager } from './LegendManager';
import { QueryAction } from './queryaction';
import { ErrorMessage } from './errormessage';
import { MapTools } from './maptools';
import { MapGeometryAction } from './mapgeometryaction';
import { FeatureCaptureComponent } from './feature-capture/feature-capture.component';
import { SearchByDrawing } from './searchbydrawing';
import { FeasibilityTool } from './feasibilityTool';
import { ConfigManager } from '../config-ang';
import XYZ from 'ol/source/XYZ';

@Component({
  selector: 'app-map-view',
  templateUrl: './map-view.component.html',
  styleUrls: ['./map-view.component.css'],
})
export class MapViewComponent implements OnInit {
  selectedLevel: any;
  scaleType = 'scalebar';
  scaleBarSteps = 4;
  scaleBarText = true;
  mmap: Map;
  LPIS: string[];
  SourceProjection = 'EPSG:4326';
  DestinationProjection = 'EPSG:3857';
  mousemovelbl: any = '';
  scaleControler: any;
  isFeatureSelectOn = false;

  @ViewChild(FeatureCaptureComponent, { static: true })
  FC!: FeatureCaptureComponent;

  constructor(
    private headerbtn: HeaderButton,
    private lpiserver: LPIService,
    private LM: LayerManager,
    private IM: IdentifyManager,
    private LEM: LegendManager,
    public QA: QueryAction,
    public FE: FeasibilityTool,
    private errMsg: ErrorMessage,
    private mga: MapGeometryAction,
    public maptool: MapTools,
    public searchbydrawing: SearchByDrawing,
    public feasibilitytool: FeasibilityTool,
    private configmanager: ConfigManager
  ) {
    this.mmap = new Map({});
    this.LPIS = [];
  }

  ngOnInit(): void {
    this.isFeatureSelectOn = false;
    this.InitMap();
    this.searchbydrawing.initSearchByDrawing(this.mmap);
    this.getMapLayer();
    this.LM.BindLayerManagerControl(this.mmap);
    this.LEM.BindLegendManagerControl(this.mmap);
    this.QA.actionOnQueryString(this.mmap);
    this.IM.InitIdentify(this.mmap);
    this.maptool.InitMapTools(this.mmap);
    this.feasibilitytool.BindFeasibilityControl(this.mmap, this);
    this.OnHeaderButtonClick('boxSearchByDrawing');
  }

  OnBasemapButtonClick(par: any) {
    var lyrs = this.mmap.getLayers().getArray();
    for (let i = 0; i < lyrs.length; i++) {
      var lly = lyrs[i];
      var tCls = lly.getClassName();

      var b = tCls.split('___')[0];
      var lName = tCls.split('___')[1];
      //var tyyyy =  lly.setVisible(false);
      if (b == 'b') {
        if (lName == par) {
          lly.setVisible(true);
          this.mga.SetCurrentBaseMap(par);
        } else {
          lly.setVisible(false);
        }
      }
      //var vis = lly.getVisible();
    }
  }

  openBulkFeasibility() {
    window.open(this.configmanager.BulkData, '_blank');
  }

  //#region Header Menu click
  OnHeaderButtonClick(buttonName: any) {
    var hdnIsFeatureSelectOn: any = document.getElementById(
      'hdnIsFeatureSelectOn'
    )!;
    if (hdnIsFeatureSelectOn.value == 'TRUE') {
      return;
    }

    this.headerbtn.onHeaderBtnclick(buttonName);
    this.OnAllButtonClickBoxBottom();
  }
  OnHeaderButtonClick_FeasibilityTool(buttonName: any) {

    this.mga.ClearAllMemoryLayerGeometry();
    var txtAddressGoogleSearch: any =
      document.getElementById('txtAddressGoogleSearch')!;
    txtAddressGoogleSearch.value = '';
    const chkWalking = document.getElementById("chkWalking") as HTMLInputElement;
    chkWalking.checked = true;
    const chkDriving = document.getElementById("chkDriving") as HTMLInputElement;
    chkDriving.checked = false;
    const chkUseMaxDistance = document.getElementById("chkUseMaxDistance") as HTMLInputElement;
    chkUseMaxDistance.checked = false;

    const chkDDMaxDistance = document.getElementById("chkDDMaxDistance") as HTMLInputElement;
    chkDDMaxDistance.checked = false;

    this.FE.fesDataForDocker = null;
            
    this.FE.dockerVisibility(false);
            
  }

  OnContainerClosedBoxBottom(containerName: any) {
    var boxName = document.getElementById('boxBottomGrid')!;
    boxName.style.display = 'none';
  }

  OnAllButtonClickBoxBottom() {
    var boxNamefES = document.getElementById('boxSearchByDrawing')!;

    var boxName = document.getElementById('boxBottomGrid')!;
    if (boxNamefES.style.display == 'block') {
      boxName.style.display = 'block';
    } else {
      boxName.style.display = 'none';
    }
  }

  OnContainerClosed(containerName: any) {
    this.headerbtn.closeAllBox();
    this.maptool.StopInteraction();
  }

  OnInfoContainerClosed() {
    this.IM.ClosedIdentifyBox();
  }



  OnMapRefresh() {
    var tyy = window.location.href;

    window.location.reload();
  }



  OnDataRefresh() {
    var lyrs = this.mmap.getLayers().getArray();
    for (let i = 0; i < lyrs.length; i++) {
      var lly = lyrs[i];
      //lly.redraw();
    }
  }

  OnValidateButtonClick(buttonName: any) {
    //this.headerbtn.baseMapButtonclick();

  }

  OnTownValidateButtonClick(buttonName: any) {

  }

  //#endregion

  InitMap() {
    this.mmap = new Map({
      target: 'map',
      controls: defaultControls().extend([
        this.scaleControl(),
        this.mousePositionControl,
      ]),
      layers: [

        new TileLayer({
          className: 'b___OSM',
          source: new OSM(),
        }),
        new TileLayer({
          className: 'b___ESRIWorldImagery',
          visible: false,
          preload: Infinity,
          source: new XYZ({
            url: 'https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}',
            attributions: '© Esri, Maxar, Earthstar Geographics, and the GIS User Community'
          })
        }),
        new TileLayer({
          className: 'b___ESRIStreets',
          visible: false,
          preload: Infinity,
          source: new XYZ({
            url: 'https://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}',
            attributions: '© Esri, Maxar, Earthstar Geographics, and the GIS User Community'
          })
        }),
        new TileLayer({
          className: 'b___CDNGI_Imagery_50cm_MOSAIC',
          visible: false,
          source: new TileWMS({
            url: 'http://apollo.cdngiportal.co.za/erdas-iws/ogc/wms/CDNGI_IMAGERY_MOSAICS',
            params: { LAYERS: 'CDNGI_Imagery_50cm_MOSAIC', TILED: true },
            // Countries have transparency, so do not fade tiles:
            transition: 0,
          }),
        }),
      ],
      view: new View({
        center: olProj.fromLonLat([25.0182595139655, -29.8625885585151]),
        zoom: 5,
      }),
    });

    this.mmap.on('singleclick', this.onMapClick);
    this.mmap.on('moveend', this.onZoomEnd);
  }

  scaleControl() {
    var control = new ScaleLine({
      units: 'metric',
      bar: true,
      steps: 4,
      text: true,
      minWidth: 140,
    });
    this.scaleControler = control;
    return control;
  }

  mousePositionControl = new MousePosition({
    coordinateFormat: createStringXY(4),
    projection: 'EPSG:4326',
    className: 'ol-mouse-position',
    //target: document.getElementById('divLocation')!
  });

  onZoomEnd(params: any) {
    const mapLocal = params.map;
    // this.mapCenterPoint = mapLocal.getView().getCenter();
    var mapZoomLevel: any = mapLocal.getView().getZoom();
    var zoom: any = document.getElementById('divZoom');

    zoom.innerHTML = 'Zoom Level : ' + Math.round(mapZoomLevel);

    var element: any = document.getElementsByClassName('ol-scale-text')[0];
    var v: any = element.innerHTML.split(':')[1].replace(',', '');
    document
      .getElementById('txtMapScale')
      ?.setAttribute('value', parseFloat(v).toFixed(0));
  }

  onMapClick(evt: any) {
    const coordinate = evt.coordinate;
    var coords = olProj.toLonLat(coordinate, 'EPSG:3857');
    console.log('Map Clicked Coordinate : ' + coords);
    var hdnIsFeatureSelectOn: any = document.getElementById(
      'hdnIsFeatureSelectOn'
    )!;
    if (hdnIsFeatureSelectOn.value == 'TRUE') {
      var mapCord = evt.pixel[0] + '|' + evt.pixel[1];
      document.getElementById('hdnVectorMapPoint')?.setAttribute('value', mapCord);
      document.getElementById('VectorPropIdClick')?.click();
      return;
    }

    var localThis = this;
    var boxName = document.getElementById('boxIdentify');
    if (
      boxName != null &&
      boxName.style.display != 'none' &&
      boxName.style.display != ''
    ) {
      try {
        var XYS = coords[0] + '|' + coords[1];
        document.getElementById('hdnMapPoint')?.setAttribute('value', XYS);
        document.getElementById('PropIdClick')?.click();
      } catch (ex) {
        console.log(ex);
      }
    }
    var boxSearchByDrawing = document.getElementById('boxSearchByDrawing');
    if (
      boxSearchByDrawing != null &&
      boxSearchByDrawing.style.display != 'none' &&
      boxSearchByDrawing.style.display != ''
    ) {
      try {
        const checkbox = document.getElementById("addressByMap") as HTMLInputElement;
        const isChecked = checkbox.checked;
        if (isChecked) {
          var XYS = coords[1] + ', ' + coords[0];
          document.getElementById('hdnVectorMapPointBluePin')?.setAttribute('value', coords[0] + '|' + coords[1]);
          var txtAddressGoogleSearch: any = document.getElementById('txtAddressGoogleSearch')!;
          txtAddressGoogleSearch.value = XYS;
          document.getElementById('btnAddressOnSearchClick')?.click();
        }
      } catch (ex) {
        console.log(ex);
      }

      try {
        const checkbox_GreenPin = document.getElementById("addressByMapGreenPin") as HTMLInputElement;
        const _GreenPin = checkbox_GreenPin.checked;
        if (_GreenPin) {
          var XYSGreen = coords[1] + ', ' + coords[0];
          const hdnGreenPin = document.getElementById('hdngreenPin')!;
          hdnGreenPin.setAttribute('value', XYSGreen);
          document.getElementById('GreenClick')?.click();
        }
      } catch (ex) {
        console.log(ex);
      }
    }
  }

  onlyOneAddressByMap(onlyOneAddressByMap: any) {
    var checkbox_Selected = document.getElementById(onlyOneAddressByMap) as HTMLInputElement;

    var IsChe = checkbox_Selected.checked;
    var checkbox_addressByMap = document.getElementById("addressByMap") as HTMLInputElement;
    var checkbox_addressByMapGreenPin = document.getElementById("addressByMapGreenPin") as HTMLInputElement;
    checkbox_addressByMap.checked = false;
    checkbox_addressByMapGreenPin.checked = false;
    if (IsChe == true)
      checkbox_Selected.checked = true;
    else IsChe = false;

    

    
  }

  OnChangeInfoLayer(evt: any) {
    this.IM.BindLayerResultAttributes(evt.target.value);
  }

  VectorFeatureSelect() {
    var hdnVectorMapPoint: any = document.getElementById('hdnVectorMapPoint')!;

  }

  parentStartIdentifyData() {
    var par: any = document.getElementById('hdnMapPoint');
    this.IM.StartIdentifyData(par.value);
  }
  parentStartCaptureData() {
    var par: any = document.getElementById('hdnMapPointForCapture');
    this.FC.CaptureDataFromCoord(par.value);
  }

  afterApplyChangeseData() {

    this.searchbydrawing.drawingStart('clear')

  }

  afterGettingSplitedData() {
    var par: any = document.getElementById('hdnSplitedPolygons');
    //alert(par.value);
    this.searchbydrawing.DrawSplitedPolygon(par.value);
  }

  getMapLayer() {
    this.lpiserver.getLMData().subscribe((layers) => {
      this.bindMapLayers(layers);
    });
  }

  bindMapLayers(layers: any) {
    var selff = this;
    layers.forEach(function (value: any) {
      var Lyr = new TileLayer({
        className: 'O___' + value.LAYER_NAME.split(':')[1],
        source: new TileWMS({
          url: value.LAYER_WMS_URL,
          params: { LAYERS: value.LAYER_NAME, TILED: true },
          serverType: 'geoserver',

          // Countries have transparency, so do not fade tiles:
          transition: 0,
        }),
      });
      selff.mmap.addLayer(Lyr);
      Lyr.setVisible(value.VISIBILITY == true);
    });
  }

  closedivErrMsg(text: any) {
    this.errMsg.closeDivErrorInfo(text);
  }

  GetTotalMRC(tsod: any) {
    var res: any;
    res = (tsod.MRC.replace(',', '.') === '--' ? 0 : parseFloat(tsod.MRC.replace(',', '.'))) + (tsod.Party_MRC_3rd.replace(',', '.') === '--' ? 0 : parseFloat(tsod.Party_MRC_3rd.replace(',', '.')));
    return res;
  }

  GetTotalNRC(tsod: any) {
    var res: any;

    res = (tsod.NRC.replace(',', '.') === '--' ? 0 : parseFloat(tsod.NRC.replace(',', '.'))) + (tsod.Party_NRC_3rd.replace(',', '.') === '--' ? 0 : parseFloat(tsod.Party_NRC_3rd.replace(',', '.'))) + this.CalculateWayleaveNRC(tsod) + this.CalculateSpecialBuildNRC(tsod);
    return res;
  }

  GetDecimal4Digit(Val: any) {
    if (Val != '--') {
      var num = parseFloat(Val.replace(',', '.'))
      return parseFloat(num.toFixed(4));
    }
    return Val;
  }

   CalculateSpecialBuildNRC(tsod: any) {
    var FinalRes: any = 0;
    var Distance_Meters_Int = tsod.Distance_Meters_Int;

    //Old Logic
    // if (tsod.chkUseMaxDistance) {
    //   if (Distance_Meters_Int > 120) {
    //     FinalRes = (Distance_Meters_Int - 120) * 438;
    //   }
    //   else { FinalRes = 0; }

    // }

    var losInt = parseFloat(tsod.Los);

    if (tsod.ChkUseMaxDistance) {
      if (Distance_Meters_Int > losInt) {
        FinalRes = (Distance_Meters_Int - losInt) * 438;
      }
      else { FinalRes = 0; }

    }
    else if (tsod.ChkDDMaxDistance) {

      if (Distance_Meters_Int > 300) {
        FinalRes = (Distance_Meters_Int - 300) * 438;
      }
      else { FinalRes = 0; }
    }
    return FinalRes;
  }

  CalculateWayleaveNRC(tsod: any) {
    var FinalRes: any = 0;
    if (tsod.IsCPTPTA) {
      FinalRes = 22802;
    }
    return FinalRes;
  }
}
