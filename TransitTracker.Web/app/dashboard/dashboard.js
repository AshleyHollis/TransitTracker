(function() {
    'use strict';
    var controllerId = 'dashboard';
    angular.module('app').controller(controllerId, ['$scope', '$timeout', '$interval', 'common', 'datacontext', 'uiGmapIsReady', dashboard]);

    function dashboard($scope, $timeout, $interval, common, datacontext, uiGmapIsReady) {
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
        proxy.client.notifyChanged = function(data) {
            //log('Update Received for' + data.VehicleId);
            var markers = vm.markers
            var markerFound = false;

            for (var i = 0; i < markers.length; i++) {
                if (markers[i].id == data.VehicleId) {
                    markerFound = true;

                    markers[i].latitude = data.Latitude;
                    markers[i].longitude = data.Longitude;
                    break;
                }
            }

            if (!markerFound) {
                var marker = {
                    "idKey": data.VehicleId,
                    "latitude": data.Latitude,
                    "longitude": data.Longitude
                }
            }

            vm.markers.push(marker);

            console.log($scope.map.markers);
        }

        $.connection.hub.start().done(function() {
            log('gpsClientHub connected', '[signalR]');
            //Calls the notify method of the server
            proxy.server.notify($.connection.hub.id);
        });

        activate();

        function activate() {
            var promises = [getMessageCount(), getPeople()];
            common.activateController(promises, controllerId)
                .then(function() { log('Activated Map View'); });
        }

        function getMessageCount() {
            return datacontext.getMessageCount().then(function(data) {
                return vm.messageCount = data;
            });
        }

        function getPeople() {
            return datacontext.getPeople().then(function(data) {
                return vm.people = data;
            });
        }

        uiGmapIsReady.promise().then(function (maps) {
            //Console.log('Map Ready MotherFucker');
            //$interval(function() { $scope.map.refresh = true }, 1000)
            $interval(function () { $scope.map.refresh = true }, 1000)
        });
    }
})();