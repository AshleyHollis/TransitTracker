'use strict';

function gpsrClientController($scope, signalRHubProxy) {
    log('gpsClientController called');
    var clientPushHubProxy = signalRHubProxy(signalRHubProxy.defaultServer, 'gpsClientHub', { logging: true });

    clientPushHubProxy.on('notifyChanged', function (data) {
        log('notifyChanged called');
        //$scope.data = data;
        Console.log(data);
        var x = clientPushHubProxy.connection.id;
    });
};