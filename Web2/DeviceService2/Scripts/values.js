var ViewModel = function () {
    var self = this;
    self.values = ko.observableArray();
    self.valuesCount = ko.observable(0);
    self.error = ko.observable();
    self.detail = ko.observable();
    self.devices = ko.observableArray();
    self.newValue = {
        Device: ko.observable(),
        Data: ko.observable(),
        Title: ko.observable()
    }

    var valuesUri = '/api/values/';
    var devicesUri = '/api/devices/';

    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        }).fail(function (jqXHR, textStatus, errorThrown) {
            self.error(errorThrown);
        });
    }

    function getAllValues() {
        ajaxHelper(valuesUri, 'GET').done(function (data) {
            self.values(data);
            self.valuesCount(data.length);
        });
    }

    self.getValueDetail = function (item) {
        ajaxHelper(valuesUri + item.Id, 'GET').done(function (data) {
            self.detail(data);
        });
    }

    function getDevices() {
        ajaxHelper(devicesUri, 'GET').done(function (data) {
            self.devices(data);
        });
    }


    self.addValue = function () {
        var value = {
            DeviceId: self.newValue.Device().Id,
            Data: self.newValue.Data(),
            Title: self.newValue.Title()
        };

        ajaxHelper(valuesUri, 'POST', value).done(function (item) {
            self.values.push(item);
        });
    }

    // Fetch the initial data.
    getAllValues();
    getDevices();
};

ko.applyBindings(new ViewModel());