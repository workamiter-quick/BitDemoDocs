import { Injectable } from '@angular/core';
import { LPIService } from './backendservice';
import { LoaderWaiting } from './loader';
import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import Style from 'ol/style/Style';
import Stroke from 'ol/style/Stroke';
import Fill from 'ol/style/Fill';
import Circle from 'ol/style/Circle';
import * as olProj from 'ol/proj';
import * as proj4x from 'proj4';
import { register } from 'ol/proj/proj4';
import Overlay from 'ol/Overlay';
import Polygon from 'ol/geom/Polygon';
import LineString from 'ol/geom/LineString';
import * as Sphere from 'ol/sphere';
import DrawMeasureVector from 'ol/interaction/draw';
import Observable from 'ol/Observable';
import { ErrorMessage } from './errormessage';
import { MapGeometryAction } from './mapgeometryaction';

@Injectable({
  providedIn: 'root',
})
export class MapTools {
  mmap: any;
  //#region MeasureTool
  vectorMeasurementLayer: any;
  sourceMeasure: any;
  sketch: any;
  helpTooltipElement: any;
  helpTooltip: any;
  continuePolygonMsg = 'Click to continue drawing the polygon';
  continueLineMsg = 'Click to continue drawing the line';
  draw: any;
  measureToolName: any = '';
  //#endregion
  constructor(
    private lpiserver: LPIService,
    private loader: LoaderWaiting,
    private errMsg: ErrorMessage,
    private mga: MapGeometryAction
  ) {}

  InitMapTools(_map: any) {
    this.mmap = _map;
    this.OnRegisterProjection();
    this.CreateVectorLayerForMeasure();
  }

  OnRegisterProjection() {
    let proj4 = (proj4x as any).default;
    proj4.defs(
      'EPSG:2046',
      '+proj=tmerc +lat_0=0 +lon_0=15 +k=1 +x_0=0 +y_0=0 +axis=wsu +ellps=WGS84 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs'
    );
    proj4.defs(
      'EPSG:2047',
      '+proj=tmerc +lat_0=0 +lon_0=17 +k=1 +x_0=0 +y_0=0 +axis=wsu +ellps=WGS84 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs'
    );
    proj4.defs(
      'EPSG:2048',
      '+proj=tmerc +lat_0=0 +lon_0=19 +k=1 +x_0=0 +y_0=0 +axis=wsu +ellps=WGS84 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs'
    );
    proj4.defs(
      'EPSG:2049',
      '+proj=tmerc +lat_0=0 +lon_0=21 +k=1 +x_0=0 +y_0=0 +axis=wsu +ellps=WGS84 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs'
    );
    proj4.defs(
      'EPSG:2050',
      '+proj=tmerc +lat_0=0 +lon_0=23 +k=1 +x_0=0 +y_0=0 +axis=wsu +ellps=WGS84 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs'
    );
    proj4.defs(
      'EPSG:2051',
      '+proj=tmerc +lat_0=0 +lon_0=25 +k=1 +x_0=0 +y_0=0 +axis=wsu +ellps=WGS84 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs'
    );
    proj4.defs(
      'EPSG:2052',
      '+proj=tmerc +lat_0=0 +lon_0=27 +k=1 +x_0=0 +y_0=0 +axis=wsu +ellps=WGS84 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs'
    );
    proj4.defs(
      'EPSG:2053',
      '+proj=tmerc +lat_0=0 +lon_0=29 +k=1 +x_0=0 +y_0=0 +axis=wsu +ellps=WGS84 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs'
    );
    proj4.defs(
      'EPSG:2054',
      '+proj=tmerc +lat_0=0 +lon_0=31 +k=1 +x_0=0 +y_0=0 +axis=wsu +ellps=WGS84 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs'
    );
    proj4.defs(
      'EPSG:2055',
      '+proj=tmerc +lat_0=0 +lon_0=33 +k=1 +x_0=0 +y_0=0 +axis=wsu +ellps=WGS84 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs'
    );
    register(proj4);
  }

  onMapToolsBtnclick(par: any) {
    this.closeAllMapTools();
    var boxName = document.getElementById(par);
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

  closeAllMapTools() {
    var boxName = document.getElementById('ctrlToolMapScaleBox');
    if (boxName != null) {
      boxName.style.display = 'none';
    }
    boxName = document.getElementById('ctrlToolAreaMeasureBox');
    if (boxName != null) {
      boxName.style.display = 'none';
    }
    boxName = document.getElementById('ctrlToolDistanceMeasureBox');
    if (boxName != null) {
      boxName.style.display = 'none';
    }
    boxName = document.getElementById('ctrlToolPerimeterMeasureBox');
    if (boxName != null) {
      boxName.style.display = 'none';
    }
    boxName = document.getElementById('ctrlToolZoomtoCoordinates');
    if (boxName != null) {
      boxName.style.display = 'none';
    }
    boxName = document.getElementById('ctrlToolShareViewBox');
    if (boxName != null) {
      boxName.style.display = 'none';
    }
    boxName = document.getElementById('ctrlToolAddressSearch');
    if (boxName != null) {
      boxName.style.display = 'none';
    }
  }

  //#region Zoom to Coordinate
  selectedProj: any = 'DMS';
  onZoomToCoordinate() {
    if (this.selectedProj == 'DMS') {
      var pnlDMS = document.getElementById('pnlDMS');
      if (pnlDMS != null) {
        var txtLATDegrees = (<HTMLInputElement>(
          document.getElementById('txtLATDegrees')
        )).value;
        var txtLATMin = (<HTMLInputElement>document.getElementById('txtLATMin'))
          .value;
        var txtLATSec = (<HTMLInputElement>document.getElementById('txtLATSec'))
          .value;

        var LAT =
          parseFloat(txtLATDegrees) +
          parseFloat(txtLATMin) / 60 +
          parseFloat(txtLATSec) / 3600;

        var txtLONDegrees = (<HTMLInputElement>(
          document.getElementById('txtLONDegrees')
        )).value;
        var txtLONMin = (<HTMLInputElement>document.getElementById('txtLONMin'))
          .value;
        var txtLONSec = (<HTMLInputElement>document.getElementById('txtLONSec'))
          .value;

        var LON =
          parseFloat(txtLONDegrees) +
          parseFloat(txtLONMin) / 60 +
          parseFloat(txtLONSec) / 3600;

        this.mga.zoomToXY(LON.toString(), LAT.toString());
      }
    }
    if (this.selectedProj != 'DMS') {
      var pnlNonDMS = document.getElementById('pnlNonDMS');
      if (pnlNonDMS != null) {
        var txtXCoord = (<HTMLInputElement>document.getElementById('txtXCoord'))
          .value;
        var txtYCoord = (<HTMLInputElement>document.getElementById('txtYCoord'))
          .value;

        this.CalculateCoordinatesAnZoom(
          txtXCoord,
          txtYCoord,
          this.selectedProj
        );
      }
    }
  }
  CalculateCoordinatesAnZoom(Xcoord: any, Ycoord: any, PrjType: any) {
    var epsg = 'EPSG:4326';
    var newCoord = olProj.transform(
      [parseFloat(Xcoord), parseFloat(Ycoord)],
      epsg,
      'EPSG:4326'
    );
    try {
      //
      switch (PrjType) {
        case 'LO15':
          epsg = 'EPSG:2046';
          newCoord = olProj.transform(
            [parseFloat(Xcoord), parseFloat(Ycoord)],
            epsg,
            'EPSG:4326'
          );
          break;
        case 'LO17':
          epsg = 'EPSG:2047';
          newCoord = olProj.transform(
            [parseFloat(Xcoord), parseFloat(Ycoord)],
            epsg,
            'EPSG:4326'
          ); //x = 1412807.8402, y = -3209199.9874
          break;
        case 'LO19':
          epsg = 'EPSG:2048';
          newCoord = olProj.transform(
            [parseFloat(Xcoord), parseFloat(Ycoord)],
            epsg,
            'EPSG:4326'
          );
          break;
        case 'LO21':
          epsg = 'EPSG:2049';
          newCoord = olProj.transform(
            [parseFloat(Xcoord), parseFloat(Ycoord)],
            epsg,
            'EPSG:4326'
          );
          break;
        case 'LO23':
          epsg = 'EPSG:2050';
          newCoord = olProj.transform(
            [parseFloat(Xcoord), parseFloat(Ycoord)],
            epsg,
            'EPSG:4326'
          );
          break;
        case 'LO25':
          epsg = 'EPSG:2051';
          newCoord = olProj.transform(
            [parseFloat(Xcoord), parseFloat(Ycoord)],
            epsg,
            'EPSG:4326'
          );
          break;
        case 'LO27':
          epsg = 'EPSG:2052';
          newCoord = olProj.transform(
            [parseFloat(Xcoord), parseFloat(Ycoord)],
            epsg,
            'EPSG:4326'
          );
          break;
        case 'LO29':
          epsg = 'EPSG:2053';
          newCoord = olProj.transform(
            [parseFloat(Xcoord), parseFloat(Ycoord)],
            epsg,
            'EPSG:4326'
          );
          break;
        case 'LO31':
          epsg = 'EPSG:2054';
          newCoord = olProj.transform(
            [parseFloat(Xcoord), parseFloat(Ycoord)],
            epsg,
            'EPSG:4326'
          );
          break;
        case 'LO33':
          epsg = 'EPSG:2055';
          newCoord = olProj.transform(
            [parseFloat(Xcoord), parseFloat(Ycoord)],
            epsg,
            'EPSG:4326'
          );
          break;
        //Longitude is X and Latitude is Y for WGS84
      }
      console.log(newCoord);
      this.mga.zoomToXY(newCoord[0].toString(), newCoord[1].toString());
    } catch (ex) {
      this.errMsg.ShowErrorMessage(ex);
    }
  }
  OnChangeCoordinatesDropDown(par: any) {
    //alert(par.target.value);
    this.selectedProj = par.target.value;
    var pnlDMS = document.getElementById('pnlDMS');
    if (pnlDMS != null) {
      pnlDMS.style.display = 'none';
    }
    var pnlNonDMS = document.getElementById('pnlNonDMS');
    if (pnlNonDMS != null) {
      pnlNonDMS.style.display = 'none';
    }
    var lblXcoord = document.getElementById('lblXcoord');
    if (lblXcoord != null) {
      lblXcoord.innerHTML = 'X';
    }
    var lblYcoord = document.getElementById('lblYcoord');
    if (lblYcoord != null) {
      lblYcoord.innerHTML = 'Y';
    }

    if (par.target.value == 'DMS') {
      if (pnlDMS != null) {
        pnlDMS.style.display = 'block';
      }
    } else if (par.target.value == 'WGS84') {
      if (pnlNonDMS != null) {
        pnlNonDMS.style.display = 'block';
      }
      if (lblXcoord != null) {
        lblXcoord.innerHTML = 'Longitude';
      }
      if (lblYcoord != null) {
        lblYcoord.innerHTML = 'Latitude';
      }
    } else {
      if (pnlNonDMS != null) {
        pnlNonDMS.style.display = 'block';
      }
      if (lblXcoord != null) {
        lblXcoord.innerHTML = 'Y Coordinate';
      }
      if (lblYcoord != null) {
        lblYcoord.innerHTML = 'X Coordinate';
      }
    }
  }
  //#endregion

  //#region Measure tool implimentation
  CreateVectorLayerForMeasure() {
    this.sourceMeasure = new VectorSource();

    this.vectorMeasurementLayer = new VectorLayer({
      source: this.sourceMeasure,
      style: new Style({
        fill: new Fill({
          color: 'rgba(255, 255, 255, 0.2)',
        }),
        stroke: new Stroke({
          color: '#ffcc33',
          width: 2,
        }),
        image: new Circle({
          radius: 7,
          fill: new Fill({
            color: '#ffcc33',
          }),
        }),
      }),
    });
    this.mmap.addLayer(this.vectorMeasurementLayer);
  }

  createHelpTooltip() {
    if (this.helpTooltipElement) {
      this.helpTooltipElement.parentNode.removeChild(this.helpTooltipElement);
    }
    this.helpTooltipElement = document.createElement('div');
    this.helpTooltipElement.className = 'ol-tooltip hidden';
    this.helpTooltip = new Overlay({
      element: this.helpTooltipElement,
      offset: [15, 0],
      //position: 'center-left',
    });
    this.mmap.addOverlay(this.helpTooltip);
  }

  AddButtonInteraction(type: any) {
    //this.StopInteraction();
    this.draw = new DrawMeasureVector({
      source: this.sourceMeasure,
      type: type,
      style: new Style({
        fill: new Fill({
          color: 'rgba(255, 255, 255, 0.2)',
        }),
        stroke: new Stroke({
          color: 'rgba(0, 0, 0, 0.5)',
          lineDash: [10, 10],
          width: 2,
        }),
        image: new Circle({
          radius: 5,
          stroke: new Stroke({
            color: 'rgba(0, 0, 0, 0.7)',
          }),
          fill: new Fill({
            color: 'rgba(255, 255, 255, 0.2)',
          }),
        }),
      }),
    });
    this.mmap.addInteraction(this.draw);
    var that = this;
    //createMeasureTooltip();
    this.createHelpTooltip();
    let listener: any;
    this.draw.on('drawstart', function (evt: any) {
      // set sketch
      that.sketch = evt.feature;

      /** @type {import("../src/ol/coordinate.js").Coordinate|undefined} */
      let tooltipCoord = evt.coordinate;

      listener = that.sketch.getGeometry().on('change', function (evt: any) {
        const geom = evt.target;
        var output: any;
        if (geom instanceof Polygon) {
          output = that.formatArea(geom);
          tooltipCoord = geom.getInteriorPoint().getCoordinates();
        } else if (geom instanceof LineString) {
          output = that.formatLength(geom);
          tooltipCoord = geom.getLastCoordinate();
        }
        if (that.measureToolName == 'Polygon') {
          var dMeasureArea = document.getElementById('dMeasureArea');
          if (dMeasureArea != null) {
            dMeasureArea.innerHTML = output;
          }
        } else if (that.measureToolName == 'LineString') {
          var dMeasureDistace = document.getElementById('dMeasureDistace');
          if (dMeasureDistace != null) {
            dMeasureDistace.innerHTML = output;
          }
        } else if (that.measureToolName == 'Perimeter') {
          var dMeasurePerimeter = document.getElementById('dMeasurePerimeter');
          if (dMeasurePerimeter != null) {
            dMeasurePerimeter.innerHTML = output;
          }
        }
      });
    });
    this.draw.on('drawend', function () {
      that.sketch = null;

      //Observable.unByKey(listener);
    });
  }

  StopInteraction() {
    this.mmap.removeInteraction(this.draw);
    this.mmap.removeOverlay(this.helpTooltip);
    this.sourceMeasure.clear();
    this.measureToolName = '';
    this.draw = null;
  }

  formatLength(line: any) {
    const length = Sphere.getLength(line);
    let output;
    if (length > 100) {
      output = Math.round((length / 1000) * 100) / 100 + ' ' + 'km';
    } else {
      output = Math.round(length * 100) / 100 + ' ' + 'm';
    }
    return output;
  }

  formatArea(polygon: any) {
    const area = Sphere.getArea(polygon);
    let output;
    if (area > 10000) {
      output =
        Math.round((area / 1000000) * 100) / 100 + ' ' + 'km<sup>2</sup>';
    } else {
      output = Math.round(area * 100) / 100 + ' ' + 'm<sup>2</sup>';
    }
    return output;
  }

  pointerMoveHandler(evt: any) {
    if (evt.dragging) {
      return;
    }

    let helpMsg = 'Click to start drawing';

    if (this.sketch) {
      const geom = this.sketch.getGeometry();
      if (geom instanceof Polygon) {
        helpMsg = this.continuePolygonMsg;
      } else if (geom instanceof LineString) {
        helpMsg = this.continueLineMsg;
      }
    }

    this.helpTooltipElement.innerHTML = helpMsg;
    this.helpTooltip.setPosition(evt.coordinate);

    this.helpTooltipElement.classList.remove('hidden');
  }
  //#endregion

  OnMapToolsButtonClick(buttonName: any) {
    this.onMapToolsBtnclick(buttonName);
    this.StopInteraction();
    if (buttonName == 'ctrlToolAreaMeasureBox') {
      this.measureToolName = 'Polygon';
      this.AddButtonInteraction('Polygon');
    }
    if (buttonName == 'ctrlToolDistanceMeasureBox') {
      this.measureToolName = 'LineString';
      this.AddButtonInteraction('LineString');
    }
    if (buttonName == 'ctrlToolPerimeterMeasureBox') {
      this.measureToolName = 'Perimeter';
      this.AddButtonInteraction('LineString');
    }
  }

  onZoomToScale() {
    var par: any = document.getElementById('txtMapScale');

    var ZoomLevel = this.GetLoadZoomScale();
    //alert(par.value);
    var cen: any = this.mmap.getView().getCenter();
    var zoo: any = this.mmap.getView().getZoom();
    for (var i = 0; i < ZoomLevel.length; i++) {
      if (par.value > ZoomLevel[i]) {
        this.mmap.getView().setCenter(cen);
        this.mmap.getView().setZoom(i);
        break;
      }
    }
  }

  GetLoadZoomScale() {
    var ZoomLevel = [];
    ZoomLevel[0] = 363864000;
    ZoomLevel[1] = 181932000;
    ZoomLevel[2] = 90966000;
    ZoomLevel[3] = 60674000;
    ZoomLevel[4] = 30337000;
    ZoomLevel[5] = 15166000;
    ZoomLevel[6] = 7584000;
    ZoomLevel[7] = 3791000;
    ZoomLevel[8] = 1896000;
    ZoomLevel[9] = 948032;
    ZoomLevel[10] = 473996;
    ZoomLevel[11] = 237010;
    ZoomLevel[12] = 118359;
    ZoomLevel[13] = 59179;
    ZoomLevel[14] = 29600;
    ZoomLevel[15] = 14795;
    ZoomLevel[16] = 7397;
    ZoomLevel[17] = 3700;
    ZoomLevel[18] = 1845;
    ZoomLevel[19] = 925;
    ZoomLevel[20] = 462;
    ZoomLevel[21] = 231;
    ZoomLevel[22] = 120;
    ZoomLevel[23] = 58;
    ZoomLevel[24] = 30;
    ZoomLevel[25] = 14;
    ZoomLevel[26] = 7;
    ZoomLevel[27] = 4;
    ZoomLevel[28] = 2;

    return ZoomLevel;
  }

  GetBingAddressSearch() {
    var dfsdf = 0;
    var par: any = document.getElementById('txtAddressGoogle');
    var fullADdd = par.value + ', South Africa';
    var that = this;
    this.lpiserver.getBingAddressSearch(fullADdd).subscribe((Pdata: any) => {
      //console.warn(Pdata);
      var OptResult: any = '<b>Result</b>';

      var divAddrssSearchResult = document.getElementById(
        'ctrlAddrssSearchResult'
      );

      if (divAddrssSearchResult != null) {
        divAddrssSearchResult.innerHTML = '';

        // var labell = document.createElement('label');
        // labell.innerHTML = OptResult;
        // divAddrssSearchResult.appendChild(labell);

        var br = document.createElement('br');
        divAddrssSearchResult.appendChild(br);
      }

      if (divAddrssSearchResult != null) {
        Pdata.resourceSets[0].resources.forEach(function (value: any) {
          var adminDistrict2 = value.address.adminDistrict2;
          var locality = value.address.locality;
          var address = locality + ', ' + adminDistrict2;
          var Y = value.geocodePoints[0].coordinates[0];
          var X = value.geocodePoints[0].coordinates[1];
          

          if (locality != undefined && adminDistrict2 != undefined) {
            if (divAddrssSearchResult != null) {
              var button = document.createElement('BUTTON');
              button.innerHTML = address;
              button.className = 'btn btn-secondary';
              button.style.marginBottom = '3px';
              button?.addEventListener('click', function handleClick(event) {
                that.mga.zoomToXY(X.toString(), Y.toString());              
              });
              divAddrssSearchResult.appendChild(button);
              var br1 = document.createElement('br');
              divAddrssSearchResult.appendChild(br1);
            }
          }
        });
      }
    });
  }

  ResultAddressClickqww() {
    alert('test');
  }
}
