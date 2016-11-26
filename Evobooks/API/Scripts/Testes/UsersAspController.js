//1.
app.controller('usersaspcontroller', function ($scope, usersaspservice) {
    $scope.Usuarios = [];

    $scope.Message = "";
    //$scope.userName = sessionStorage.getItem('userName');


    loadUsuarios();

    function loadUsuarios() {
        

        var promise = usersaspservice.get();
        promise.then(function (resp) {
            $scope.Usuarios = resp.data;
            $scope.Message = "Call Completed Successfully";
        }, function (err) {
            $scope.Message = "Error!!! " + err.status
        });
    };
});