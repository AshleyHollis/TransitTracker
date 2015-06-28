(function () {
    'use strict';
    var controllerId = 'dashboard';
    angular.module('app').controller(controllerId, ['$scope', '$timeout', 'common', 'datacontext', dashboard]);

    function dashboard($scope, $timeout ,common, datacontext, uiGmapGoogleMapApi) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.news = {
            title: 'Transit Tracker',
            description: 'Transit Tracker Website'
        };

        vm.markers = [];

        vm.messageCount = 0;
        vm.people = [];
        vm.title = 'Dashboard';
        $scope.map = { center: { latitude: -25.2743980, longitude: 133.7751360 }, zoom: 5, markers: [] };
        //SignalR Connection
        var proxy = $.connection.gpsClientHub;
        proxy.client.notifyChanged = function (data) {

            log('Update Received');
            
            var marker = {
                "id": data.VehicleId,
                "coords": {
                    "latitude": data.Latitude,
                    "longitude": data.Longitude
                },
                "window": {
                    "title": data.VehicleId
                }
            }

            //vm.markers.push(marker);

            $scope.map.markers.push(marker);
            console.log($scope.map.markers);
        }
        $.connection.hub.start().done(function () {
            log('gpsClientHub connected', '[signalR]');
            //Calls the notify method of the server
            proxy.server.notify($.connection.hub.id);
        });


        activate();

        function activate() {
            var promises = [getMessageCount(), getPeople()];
            common.activateController(promises, controllerId)
                .then(function () { log('Activated Map View'); });
        }

        function getMessageCount() {
            return datacontext.getMessageCount().then(function (data) {
                return vm.messageCount = data;
            });
        }

        function getPeople() {
            return datacontext.getPeople().then(function (data) {
                return vm.people = data;
            });
        }

        //uiGmapGoogleMapApi.then(function (maps) {
        //    Console.log('Map Ready MotherFucker');
        //});
    }
})();