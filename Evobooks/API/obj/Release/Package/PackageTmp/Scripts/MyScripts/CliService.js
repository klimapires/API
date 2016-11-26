//1.
app.service('cliservice', function ($http) {
    this.get = function () {
        
        var accesstoken = sessionStorage.getItem('accessToken');

        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }

        var response = $http({
            url: "/api/ClientesAPI",
            method: "GET",
            headers: authHeaders
        });
        return response;
    };
});
