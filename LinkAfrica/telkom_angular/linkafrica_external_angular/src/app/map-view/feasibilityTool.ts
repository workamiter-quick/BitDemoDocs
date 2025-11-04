import { Injectable } from '@angular/core';
import { LPIService } from './backendservice';
import { LoaderWaiting } from './loader';
import { AddressSearchBean, GoogleCoordBean, RoutInputCoord } from './AppBeans';
import { MapGeometryAction } from './mapgeometryaction';

@Injectable({
    providedIn: 'root',
})

export class FeasibilityTool {
    mmap: any;
    ASB: any;
    ParentCtrl: any;
    fesDataForDocker: any;
    constructor(
        private lpiserver: LPIService,
        private loader: LoaderWaiting,
        private mga: MapGeometryAction
    ) { }

    BindFeasibilityControl(_map: any, _parent: any) {
        this.mmap = _map;
        this.ParentCtrl = _parent;
    }

    searchAddressOnSearchClick() {
        this.loader.StartLoader();
        var txtAddressGoogleSearch: any =
            document.getElementById('txtAddressGoogleSearch')!;
        var value = txtAddressGoogleSearch.value;

        this.ASB = new AddressSearchBean();
        this.ASB.Address = txtAddressGoogleSearch.value;
        console.log('Enter key pressed. Searching for address:', this.ASB.Address);
        this.lpiserver.getGoogleAddressResult(this.ASB).subscribe(
            (res) => {
                console.log('Success after result:', res);
                //alert(JSON.stringify(res)); // or alert(res) if it's a string
                this.mga.ClearAllMemoryLayerGeometry();
                var GCB = new GoogleCoordBean();

                var hdnVectorMapPointBluePin: any = document.getElementById('hdnVectorMapPointBluePin')!;
                if (hdnVectorMapPointBluePin.value != '0') {
                    var blueClickPin = hdnVectorMapPointBluePin.value.split('|');
                    this.mga.zoomToXY(blueClickPin[0], blueClickPin[1]);
                    GCB.Longitude_X = blueClickPin[0];
                    GCB.Latitude_Y = blueClickPin[1];
                }
                else {
                    this.mga.zoomToXY(res.Longitude.toString(), res.Latitude.toString());
                    GCB.Longitude_X = res.Longitude.toString();
                    GCB.Latitude_Y = res.Latitude.toString();
                }
                const chkUseMaxDistance = document.getElementById("chkUseMaxDistance") as HTMLInputElement;
                const chkDDMaxDistance = document.getElementById("chkDDMaxDistance") as HTMLInputElement;
                GCB.DDMaxDistance = chkDDMaxDistance.checked;
                GCB.UseMaxDistance = chkUseMaxDistance.checked;

                const ddlBandwidth = document.getElementById("ddlBandwidth") as HTMLSelectElement;
                const ddlBandwidthValue = ddlBandwidth.value;

                const ddlContractTerm = document.getElementById("ddlContractTerm") as HTMLSelectElement;
                const ddlContractTermalue = ddlContractTerm.value;

                GCB.Bandwidth = ddlBandwidthValue;
                GCB.ContractTerm = ddlContractTermalue;


                this.lpiserver.getSiteDetailResult(GCB).subscribe(
                    (res) => {
                        console.log(JSON.stringify(res) + " : output getSiteDetailResult");
                        this.fesDataForDocker = res;
                        this.dockerVisibility(true);
                        if (res.length != 0) {
                            var hdnVectorMapPointBluePin: any = document.getElementById('hdnVectorMapPointBluePin')!;
                            var blueClickPin = hdnVectorMapPointBluePin.value.split('|');
                            if (this.fesDataForDocker[0].Longitude_X.toString().replace(',', '.') != blueClickPin[0]
                                && this.fesDataForDocker[1].Latitude_Y.toString().replace(',', '.') != blueClickPin[1]) {
                                this.mga.zoomToXYGreen(this.fesDataForDocker[0].Longitude_X.toString().replace(',', '.'),
                                    this.fesDataForDocker[0].Latitude_Y.toString().replace(',', '.'));
                            }


                            //this.mga.zoomToXYGreen(this.fesDataForDocker[0].Longitude_X.toString().replace(',', '.'), this.fesDataForDocker[0].Latitude_Y.toString().replace(',', '.'));
                            if (this.fesDataForDocker.length > 0) {
                                this.dockerVisibility(true);
                                const chkWalking = document.getElementById("chkWalking") as HTMLInputElement;
                                const chkDriving = document.getElementById("chkDriving") as HTMLInputElement;
                                var RIC = new RoutInputCoord()
                                RIC.EndLat_Y = GCB.Latitude_Y.toString().replace(',', '.');
                                RIC.EndLng_X = GCB.Longitude_X.toString().replace(',', '.');
                                RIC.StartLat_Y = this.fesDataForDocker[0].Latitude_Y.toString().replace(',', '.');
                                RIC.StartLng_X = this.fesDataForDocker[0].Longitude_X.toString().replace(',', '.');
                                RIC.Route_Mode = chkWalking.checked == true ? 'W' : (chkDriving.checked == true ? 'D' : 'W');
                                var thatt = this;
                                if (this.fesDataForDocker[0].PolygonCount === '0') {
                                    this.lpiserver.getRouteDataResult(RIC).subscribe(
                                        (resRoute) => {
                                            var LineWKKT = "LINESTRING (";

                                            var hdnVectorMapPointBluePin: any = document.getElementById('hdnVectorMapPointBluePin')!;
                                            if (hdnVectorMapPointBluePin.value != '0') {
                                                var blueClickPin = hdnVectorMapPointBluePin.value.split('|');
                                                LineWKKT = "LINESTRING (" + blueClickPin[0] + ' ' + blueClickPin[1] + ', ';
                                                hdnVectorMapPointBluePin.value = '0';
                                            }

                                            LineWKKT = LineWKKT + RIC.EndLng_X + ' ' + RIC.EndLat_Y + ', ';
                                            if (resRoute.Coords.length > 2) {
                                                resRoute.Coords.forEach(function (SEData: any) {
                                                    LineWKKT = LineWKKT + SEData.Item2.toString() + ' ' + SEData.Item1.toString() + ', ';
                                                })
                                            }
                                            //LineWKKT = LineWKKT + resRoute.Coords[resRoute.Coords.length - 1].Item2.toString() + ' ' + resRoute.Coords[resRoute.Coords.length - 1].Item1.toString() + ')';
                                            LineWKKT = LineWKKT + RIC.StartLng_X + ' ' + RIC.StartLat_Y + ')';
                                            thatt.mga.drawSingleWKT(LineWKKT);
                                            for (let i = 0; i < this.fesDataForDocker.length; i++) {
                                                var distance = resRoute.Distance.split(' ')[0];
                                                var unit = resRoute.Distance.split(' ')[1];
                                                if (unit === 'm') {
                                                    this.fesDataForDocker[i].Distance_Meters = (parseFloat(distance)) + ' meters';
                                                    this.fesDataForDocker[i].Distance_Meters_Int = parseFloat(distance);
                                                }
                                                else if (unit === 'km') {
                                                    this.fesDataForDocker[i].Distance_Meters = (parseFloat(distance) * 1000) + ' meters';
                                                    this.fesDataForDocker[i].Distance_Meters_Int = parseFloat(distance) * 1000;
                                                }

                                                this.fesDataForDocker[i].chkUseMaxDistance = chkUseMaxDistance.checked;
                                                this.fesDataForDocker[i].chkDDMaxDistance = chkDDMaxDistance.checked;
                                            }
                                        },
                                        (errRoute) => {
                                            console.error('Error:', errRoute);
                                            //alert('Something went wrong!');
                                            this.loader.StopLoader();
                                        });
                                }
                            }
                        }
                        this.loader.StopLoader();
                    },
                    (err) => {
                        this.dockerVisibility(false);
                        console.error('Error:', err);
                        //alert('Something went wrong!');
                        this.loader.StopLoader();
                    }
                )
                this.loader.StopLoader();
            },
            (err) => {
                console.error('Error:', err);
                //alert('Something went wrong!');
                this.loader.StopLoader();
            }
        );
    }

    calculateDistance(dis: any) {
        var res: any;
        if (dis > 120) {
            res = dis - 120;
        }
        return res;
    }

    searchAddressOnKeyUp(e: KeyboardEvent) {
        if (e.key === 'Enter') {
            this.loader.StartLoader();
            var txtAddressGoogleSearch: any =
                document.getElementById('txtAddressGoogleSearch')!;
            var value = txtAddressGoogleSearch.value;
            this.ASB = new AddressSearchBean();
            this.ASB.Address = txtAddressGoogleSearch.value;
            console.log('Enter key pressed. Searching for address:', this.ASB.Address);
            this.lpiserver.getGoogleAddressResult(this.ASB).subscribe(
                (res) => {
                    console.log('Success:', res);

                    //alert(JSON.stringify(res)); // or alert(res) if it's a string
                    this.mga.zoomToXY(res.Longitude.toString(), res.Latitude.toString());

                    var GCB = new GoogleCoordBean();

                    GCB.Longitude_X = res.Longitude.toString();
                    GCB.Latitude_Y = res.Latitude.toString();
                    const ddlBandwidth = document.getElementById("ddlBandwidth") as HTMLSelectElement;
                    const ddlBandwidthValue = ddlBandwidth.value;

                    const ddlContractTerm = document.getElementById("ddlContractTerm") as HTMLSelectElement;
                    const ddlContractTermalue = ddlContractTerm.value;

                    GCB.Bandwidth = ddlBandwidthValue;
                    GCB.ContractTerm = ddlContractTermalue;
                    this.lpiserver.getSiteDetailResult(GCB).subscribe(
                        (res) => {
                            console.log(JSON.stringify(res));
                            this.fesDataForDocker = res;
                            this.mga.zoomToXYGreen(this.fesDataForDocker[0].Longitude_X.toString().replace(',', '.'), this.fesDataForDocker[0].Latitude_Y.toString().replace(',', '.'));
                            if (this.fesDataForDocker.length > 0) {
                                this.dockerVisibility(true);
                                const chkWalking = document.getElementById("chkWalking") as HTMLInputElement;
                                const chkDriving = document.getElementById("chkDriving") as HTMLInputElement;
                                var RIC = new RoutInputCoord()
                                RIC.EndLat_Y = GCB.Latitude_Y.toString().replace(',', '.');
                                RIC.EndLng_X = GCB.Longitude_X.toString().replace(',', '.');
                                RIC.StartLat_Y = this.fesDataForDocker[0].Latitude_Y.toString().replace(',', '.');
                                RIC.StartLng_X = this.fesDataForDocker[0].Longitude_X.toString().replace(',', '.');
                                RIC.Route_Mode = chkWalking.checked == true ? 'W' : (chkDriving.checked == true ? 'D' : 'W');
                                var thatt = this;
                                if (this.fesDataForDocker[0].PolygonCount === '0') {
                                    this.lpiserver.getRouteDataResult(RIC).subscribe(
                                        (resRoute) => {
                                            var LineWKKT = "LINESTRING (";
                                            LineWKKT = LineWKKT + RIC.EndLng_X + ' ' + RIC.EndLat_Y + ', ';
                                            resRoute.Coords.forEach(function (SEData: any) {
                                                LineWKKT = LineWKKT + SEData.Item2.toString() + ' ' + SEData.Item1.toString() + ', ';
                                            })
                                            //LineWKKT = LineWKKT + resRoute.Coords[resRoute.Coords.length - 1].Item2.toString() + ' ' + resRoute.Coords[resRoute.Coords.length - 1].Item1.toString() + ')';
                                            LineWKKT = LineWKKT + RIC.StartLng_X + ' ' + RIC.StartLat_Y + ')';
                                            thatt.mga.drawSingleWKT(LineWKKT);
                                            for (let i = 0; i < this.fesDataForDocker.length; i++) {
                                                var distance = resRoute.Distance.split(' ')[0];
                                                var unit = resRoute.Distance.split(' ')[1];
                                                if (unit === 'm') {
                                                    this.fesDataForDocker[i].Distance_Meters = (parseFloat(distance)) + ' meters';
                                                }
                                                else if (unit === 'km') {
                                                    this.fesDataForDocker[i].Distance_Meters = (parseFloat(distance) * 1000) + ' meters';
                                                }

                                            }
                                        },
                                        (errRoute) => {
                                            console.error('Error:', errRoute);
                                            //alert('Something went wrong!');
                                            this.loader.StopLoader();
                                        });
                                }
                            }
                        },
                        (err) => {
                            console.error('Error:', err);
                            //alert('Something went wrong!');
                            this.loader.StopLoader();
                        }
                    )
                    this.loader.StopLoader();
                },
                (err) => {
                    console.error('Error:', err);
                    //alert('Something went wrong!');
                    this.loader.StopLoader();
                }
            );

        }
        else {
            this.fetchSuggetion();
        }
    }

    searchAddresGreen() {

        this.loader.StartLoader();
        var txtAddressGoogleSearch: any =
            document.getElementById('txtAddressGoogleSearch')!;
        var value = txtAddressGoogleSearch.value;
        this.ASB = new AddressSearchBean();
        this.ASB.Address = txtAddressGoogleSearch.value;
        console.log('Address to search:', this.ASB.Address);
        this.lpiserver.getGoogleAddressResult(this.ASB).subscribe(
            (res) => {
                console.log('Success:', res);
                //alert(JSON.stringify(res)); // or alert(res) if it's a string
                this.mga.zoomToXY(res.Longitude.toString(), res.Latitude.toString());

                var GCB = new GoogleCoordBean();

                GCB.Longitude_X = res.Longitude.toString();
                GCB.Latitude_Y = res.Latitude.toString();
                const ddlBandwidth = document.getElementById("ddlBandwidth") as HTMLSelectElement;
                const ddlBandwidthValue = ddlBandwidth.value;

                const ddlContractTerm = document.getElementById("ddlContractTerm") as HTMLSelectElement;
                const ddlContractTermalue = ddlContractTerm.value;

                GCB.Bandwidth = ddlBandwidthValue;
                GCB.ContractTerm = ddlContractTermalue;

                var hdngreenPin: any =
                    document.getElementById('hdngreenPin')!;
                var value_hdngreenPin = hdngreenPin.value;

                GCB.Latitude_Y_Green = hdngreenPin.value.split(',')[0].trim();
                GCB.Longitude_X_Green = hdngreenPin.value.split(',')[1].trim();


                this.lpiserver.getSiteDetailGreenResult(GCB).subscribe(
                    (res) => {
                        console.log(JSON.stringify(res));
                        this.fesDataForDocker = res;
                        this.mga.zoomToXYGreen(this.fesDataForDocker[0].Longitude_X.toString().replace(',', '.'), this.fesDataForDocker[0].Latitude_Y.toString().replace(',', '.'));
                        if (this.fesDataForDocker.length > 0) {
                            this.dockerVisibility(true);
                            var RIC = new RoutInputCoord()
                            RIC.StartLat_Y = this.fesDataForDocker[0].Latitude_Y.toString().replace(',', '.');
                            RIC.StartLng_X = this.fesDataForDocker[0].Longitude_X.toString().replace(',', '.');
                            RIC.EndLat_Y = GCB.Latitude_Y.toString().replace(',', '.');
                            RIC.EndLng_X = GCB.Longitude_X.toString().replace(',', '.');

                            const chkWalking = document.getElementById("chkWalking") as HTMLInputElement;
                            const chkDriving = document.getElementById("chkDriving") as HTMLInputElement;
                            RIC.Route_Mode = chkWalking.checked == true ? 'W' : (chkDriving.checked == true ? 'D' : 'W');

                            var thatt = this;
                            if (this.fesDataForDocker[0].PolygonCount === '0') {
                                this.lpiserver.getRouteDataResult(RIC).subscribe(
                                    (resRoute) => {
                                        var LineWKKT = "LINESTRING (";
                                        LineWKKT = LineWKKT + RIC.EndLng_X + ' ' + RIC.EndLat_Y + ', ';
                                        resRoute.Coords.forEach(function (SEData: any) {
                                            LineWKKT = LineWKKT + SEData.Item2.toString() + ' ' + SEData.Item1.toString() + ', ';
                                        })
                                        //LineWKKT = LineWKKT + resRoute.Coords[resRoute.Coords.length - 1].Item2.toString() + ' ' + resRoute.Coords[resRoute.Coords.length - 1].Item1.toString() + ')';
                                        LineWKKT = LineWKKT + RIC.StartLng_X + ' ' + RIC.StartLat_Y + ')';
                                        thatt.mga.drawSingleWKT(LineWKKT);
                                        for (let i = 0; i < this.fesDataForDocker.length; i++) {
                                            var distance = resRoute.Distance.split(' ')[0];
                                            var unit = resRoute.Distance.split(' ')[1];
                                            if (unit === 'm') {
                                                this.fesDataForDocker[i].Distance_Meters = (parseFloat(distance)) + ' meters';
                                            }
                                            else if (unit === 'km') {
                                                this.fesDataForDocker[i].Distance_Meters = (parseFloat(distance) * 1000) + ' meters';
                                            }

                                        }
                                    },
                                    (errRoute) => {
                                        console.error('Error:', errRoute);
                                        //alert('Something went wrong!');
                                        this.loader.StopLoader();
                                    });
                            }
                        }
                    },
                    (err) => {
                        console.error('Error:', err);
                        //alert('Something went wrong!');
                        this.loader.StopLoader();
                    }
                )
                this.loader.StopLoader();
            },
            (err) => {
                console.error('Error:', err);
                //alert('Something went wrong!');
                this.loader.StopLoader();
            }
        );


    }

    fetchSuggetion() {
        var txtAddressGoogleSearch: any =
            document.getElementById('txtAddressGoogleSearch')!;
        var value = txtAddressGoogleSearch.value;
        this.ASB = new AddressSearchBean();
        this.ASB.Address = txtAddressGoogleSearch.value;
        if (value.length > 2) {
            this.lpiserver.getAddressDataResult(this.ASB).subscribe(
                (res) => {
                    var container = document.getElementById("divSuggetion")!
                    console.log(JSON.stringify(res));
                    while (container.firstChild) {
                        container.removeChild(container.firstChild);
                    }
                    var inner_HTML = "";
                    for (let i = 0; i < res.length; i++) {
                        const label = document.createElement("label");
                        label.textContent = res[i];
                        label.className = "item-label";

                        label.addEventListener("click", function () {
                            console.log("Label clicked:", label.textContent);
                            var txtAddressGoogleSearchVal: any =
                                document.getElementById('txtAddressGoogleSearch')!;
                            txtAddressGoogleSearchVal.value = label.textContent;
                            while (container.firstChild) {
                                container.removeChild(container.firstChild);
                            }
                        });

                        container.appendChild(label);
                        container.appendChild(document.createElement("br"));

                    }
                    //container.innerHTML = inner_HTML;
                },

                (err) => {
                    console.error('Error:', err);
                    //alert('Something went wrong!');
                    this.loader.StopLoader();
                });
        }
        else {
            var container = document.getElementById("divSuggetion")!
            while (container.firstChild) {
                container.removeChild(container.firstChild);
            }
        }
    }

    dockerVisibility(par: any) {

        var tblFeaData = document.getElementById('tblFeaData')!;
        tblFeaData.style.display = 'none';
        if (par == true) {
            tblFeaData.style.display = 'block';
        }


    }
}