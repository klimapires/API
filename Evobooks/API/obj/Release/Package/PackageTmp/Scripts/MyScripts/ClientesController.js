//1.
app.controller('clicontroller', function ($scope, cliservice) {
    $scope.Clientes = [];

    $scope.Message = "";
    $scope.userName = sessionStorage.getItem('userName');


    loadClientes();

    function loadClientes() {


        var promise = cliservice.get();
        promise.then(function (resp) {
            $scope.Clientes = resp.data;
            $scope.Message = "Call Completed Successfully";
        }, function (err) {
            $scope.Message = "Error!!! " + err.status;
        });
    };

    $scope.logout = function () {
        sessionStorage.removeItem('accessToken');
        window.location.href = '/Account/LogOff';
    };
});