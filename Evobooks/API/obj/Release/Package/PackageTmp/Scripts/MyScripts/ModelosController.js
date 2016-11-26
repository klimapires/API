app.controller('modcontroller', function ($scope, modservice) {
    $scope.Modelos = [];

    $scope.Message = "";
    $scope.userName = sessionStorage.getItem('userName');


    loadModelos();
    
    function loadModelos() {
        var promise = modservice.get();
        promise.then(function (resp) {
            $scope.Modelos = resp.data;
            $scope.Message = "Call Completed Successfully";
        }, function (err) {
            $scope.Message = "Error!!! " + err.status;
        });
    };
});

