//! Copyright (c) Microsoft Corporation. All rights reserved.
var __extends = (this && this.__extends) || function (d, b) {for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];function __() { this.constructor = d; }__.prototype = b.prototype;d.prototype = new __();};
!function(e){if("object"==typeof exports)module.exports=e();else if("function"==typeof define&&define.amd)define(e);else{var f;"undefined"!=typeof window?f=window:"undefined"!=typeof global?f=global:"undefined"!=typeof self&&(f=self),f.OneDrive=e()}}(function(){var define,module,exports;return (function e(t,n,r){function s(o,u){if(!n[o]){if(!t[o]){var a=typeof require=="function"&&require;if(!u&&a)return a(o,!0);if(i)return i(o,!0);throw new Error("Cannot find module '"+o+"'")}var f=n[o]={exports:{}};t[o][0].call(f.exports,function(e){var n=t[o][1][e];return s(n?n:e)},f,f.exports,e,t,n,r)}return n[o].exports}var i=typeof require=="function"&&require;for(var o=0;o<r.length;o++)s(r[o]);return s})({1:[function(_dereq_,module,exports){
var ErrorType = _dereq_('./models/ErrorType');
var Constants = function () {
        function Constants() {
        }
        Constants.ERROR_ACCESS_DENIED = 'access_denied';
        Constants.ERROR_POPUP_OPEN = {
            errorCode: ErrorType.popupOpen,
            message: 'popup window is already open'
        };
        Constants.ERROR_WEB_REQUEST = {
            errorCode: ErrorType.webRequestFailure,
            message: 'web request failed, see console logs for details'
        };
        Constants.HTTP_GET = 'GET';
        Constants.HTTP_POST = 'POST';
        Constants.HTTP_PUT = 'PUT';
        Constants.LINKTYPE_WEB = 'webLink';
        Constants.LINKTYPE_DOWNLOAD = 'downloadLink';
        Constants.PARAM_ACCESS_TOKEN = 'access_token';
        Constants.PARAM_ERROR = 'error';
        Constants.PARAM_STATE = 'state';
        Constants.SDK_VERSION = 'js-v2.0';
        Constants.STATE_AAD_LOGIN = 'aad_tenant_login';
        Constants.STATE_AAD_PICKER = 'aad_picker';
        Constants.STATE_DISCOVERY = 'discovery';
        Constants.STATE_MSA_PICKER = 'msa_picker';
        Constants.TYPE_BOOLEAN = 'boolean';
        Constants.TYPE_FUNCTION = 'function';
        Constants.TYPE_OBJECT = 'object';
        Constants.TYPE_STRING = 'string';
        Constants.VROOM_URL = 'https://api.onedrive.com/v1.0/';
        Constants.VROOM_INT_URL = 'https://newapi.storage.live-int.com/v1.0/';
        return Constants;
    }();
module.exports = Constants;
},{"./models/ErrorType":6}],2:[function(_dereq_,module,exports){
var Constants = _dereq_('./Constants'), OneDriveApp = _dereq_('./OneDriveApp');
var OneDrive = function () {
        function OneDrive() {
        }
        OneDrive.open = function (options) {
            OneDriveApp.open(options);
        };
        OneDrive.save = function (options) {
            OneDriveApp.save(options);
        };
        OneDrive.webLink = Constants.LINKTYPE_WEB;
        OneDrive.downloadLink = Constants.LINKTYPE_DOWNLOAD;
        return OneDrive;
    }();
OneDriveApp.onloadInit();
module.exports = OneDrive;
},{"./Constants":1,"./OneDriveApp":3}],3:[function(_dereq_,module,exports){
var DomUtilities = _dereq_('./utilities/DomUtilities'), ErrorHandler = _dereq_('./utilities/ErrorHandler'), Logging = _dereq_('./utilities/Logging'), OneDriveState = _dereq_('./OneDriveState'), Picker = _dereq_('./utilities/Picker'), PickerMode = _dereq_('./models/PickerMode'), RedirectUtilities = _dereq_('./utilities/RedirectUtilities'), ResponseParser = _dereq_('./utilities/ResponseParser'), Saver = _dereq_('./utilities/Saver');
var OneDriveApp = function () {
        function OneDriveApp() {
        }
        OneDriveApp.onloadInit = function () {
            ErrorHandler.registerErrorObserver(OneDriveState.clearState);
            DomUtilities.getScriptInput();
            Logging.logMessage('initialized');
            var redirectResponse = RedirectUtilities.handleRedirect();
            if (!redirectResponse) {
                return;
            }
            var pickerResponse = ResponseParser.parsePickerResponse(redirectResponse);
            var options = redirectResponse.windowState.options;
            var optionsMode = redirectResponse.windowState.optionsMode;
            switch (optionsMode) {
            case PickerMode[PickerMode.open]:
                var picker = new Picker(options);
                if (pickerResponse.error) {
                    picker.handlePickerError(pickerResponse);
                } else {
                    picker.handlePickerSuccess(pickerResponse);
                }
                break;
            case PickerMode[PickerMode.save]:
                var saver = new Saver(options);
                if (pickerResponse.error) {
                    saver.handleSaverError(pickerResponse);
                } else {
                    saver.handleSaverSuccess(pickerResponse);
                }
                break;
            default:
                ErrorHandler.throwError('invalid value for options.mode: ' + optionsMode);
            }
        };
        OneDriveApp.open = function (options) {
            if (!OneDriveState.readyCheck()) {
                return;
            }
            if (!options) {
                ErrorHandler.throwError('missing picker options');
            }
            Logging.logMessage('open started');
            var picker = new Picker(options);
            picker.launchPicker();
        };
        OneDriveApp.save = function (options) {
            if (!OneDriveState.readyCheck()) {
                return;
            }
            if (!options) {
                ErrorHandler.throwError('missing saver options');
            }
            Logging.logMessage('save started');
            var saver = new Saver(options);
            saver.launchSaver();
        };
        return OneDriveApp;
    }();
module.exports = OneDriveApp;
},{"./OneDriveState":4,"./models/PickerMode":8,"./utilities/DomUtilities":14,"./utilities/ErrorHandler":15,"./utilities/Logging":17,"./utilities/Picker":19,"./utilities/RedirectUtilities":21,"./utilities/ResponseParser":22,"./utilities/Saver":23}],4:[function(_dereq_,module,exports){
var OneDriveState = function () {
        function OneDriveState() {
        }
        OneDriveState.clearState = function () {
            window.name = '';
            OneDriveState._isSdkReady = true;
        };
        OneDriveState.readyCheck = function () {
            if (!OneDriveState.clientIds || !OneDriveState._isSdkReady) {
                return false;
            }
            OneDriveState._isSdkReady = false;
            return true;
        };
        OneDriveState.getODCHost = function () {
            return (OneDriveState.debug ? 'live-int' : 'live') + '.com';
        };
        OneDriveState.debug = false;
        OneDriveState._isSdkReady = true;
        return OneDriveState;
    }();
module.exports = OneDriveState;
},{}],5:[function(_dereq_,module,exports){
var ApiEndpoint;
(function (ApiEndpoint) {
    ApiEndpoint[ApiEndpoint['filesV2'] = 0] = 'filesV2';
    ApiEndpoint[ApiEndpoint['other'] = 1] = 'other';
    ApiEndpoint[ApiEndpoint['vroom'] = 2] = 'vroom';
}(ApiEndpoint || (ApiEndpoint = {})));
module.exports = ApiEndpoint;
},{}],6:[function(_dereq_,module,exports){
var ErrorType;
(function (ErrorType) {
    ErrorType[ErrorType['badResponse'] = 0] = 'badResponse';
    ErrorType[ErrorType['fileReaderFailure'] = 1] = 'fileReaderFailure';
    ErrorType[ErrorType['popupOpen'] = 2] = 'popupOpen';
    ErrorType[ErrorType['unknown'] = 3] = 'unknown';
    ErrorType[ErrorType['unsupportedFeature'] = 4] = 'unsupportedFeature';
    ErrorType[ErrorType['webRequestFailure'] = 5] = 'webRequestFailure';
}(ErrorType || (ErrorType = {})));
module.exports = ErrorType;
},{}],7:[function(_dereq_,module,exports){
var CallbackInvoker = _dereq_('../utilities/CallbackInvoker'), Constants = _dereq_('../Constants'), Logging = _dereq_('../utilities/Logging'), StringUtilities = _dereq_('../utilities/StringUtilities'), TypeValidators = _dereq_('../utilities/TypeValidators');
var InvokerOptions = function () {
        function InvokerOptions(options) {
            this.openInNewWindow = TypeValidators.validateType(options.openInNewWindow, Constants.TYPE_BOOLEAN, true, true);
            this.expectGlobalFunction = !this.openInNewWindow;
            if (this.expectGlobalFunction) {
                this.cancelName = options.cancel;
                this.errorName = options.error;
            }
            var cancelCallback = TypeValidators.validateCallback(options.cancel, true, this.expectGlobalFunction);
            this.cancel = function () {
                Logging.logMessage('user cancelled operation');
                CallbackInvoker.invokeAppCallback(cancelCallback, true);
            };
            var errorCallback = TypeValidators.validateCallback(options.error, true, this.expectGlobalFunction);
            this.error = function (error) {
                Logging.logError(StringUtilities.format('error occured - code: \'{0}\', message: \'{1}\'', error.errorCode, error.message));
                CallbackInvoker.invokeAppCallback(errorCallback, true, error);
            };
        }
        return InvokerOptions;
    }();
module.exports = InvokerOptions;
},{"../Constants":1,"../utilities/CallbackInvoker":13,"../utilities/Logging":17,"../utilities/StringUtilities":24,"../utilities/TypeValidators":25}],8:[function(_dereq_,module,exports){
var PickerMode;
(function (PickerMode) {
    PickerMode[PickerMode['open'] = 0] = 'open';
    PickerMode[PickerMode['save'] = 1] = 'save';
}(PickerMode || (PickerMode = {})));
module.exports = PickerMode;
},{}],9:[function(_dereq_,module,exports){
var CallbackInvoker = _dereq_('../utilities/CallbackInvoker'), Constants = _dereq_('../Constants'), InvokerOptions = _dereq_('./InvokerOptions'), Logging = _dereq_('../utilities/Logging'), PickerMode = _dereq_('./PickerMode'), TypeValidators = _dereq_('../utilities/TypeValidators');
var VALID_LINKTYPE_VALUES = [
        Constants.LINKTYPE_DOWNLOAD,
        Constants.LINKTYPE_WEB
    ];
var PickerOptions = function (_super) {
        __extends(PickerOptions, _super);
        function PickerOptions(options) {
            _super.call(this, options);
            if (this.expectGlobalFunction) {
                this.successName = options.success;
            }
            var successCallback = TypeValidators.validateCallback(options.success, false, this.expectGlobalFunction);
            this.success = function (files) {
                Logging.logMessage('picker operation succeeded');
                CallbackInvoker.invokeAppCallback(successCallback, true, files);
            };
            this.multiSelect = TypeValidators.validateType(options.multiSelect, Constants.TYPE_BOOLEAN, true, false);
            this.linkType = TypeValidators.validateType(options.linkType, Constants.TYPE_STRING, true, Constants.LINKTYPE_DOWNLOAD, VALID_LINKTYPE_VALUES);
            this.getWebLinks = this.linkType === Constants.LINKTYPE_WEB;
        }
        PickerOptions.prototype.serializeState = function () {
            return {
                optionsMode: PickerMode[PickerMode.open],
                options: {
                    success: this.successName,
                    cancel: this.cancelName,
                    error: this.errorName,
                    linkType: this.linkType,
                    multiSelect: this.multiSelect,
                    openInNewWindow: this.openInNewWindow
                }
            };
        };
        return PickerOptions;
    }(InvokerOptions);
module.exports = PickerOptions;
},{"../Constants":1,"../utilities/CallbackInvoker":13,"../utilities/Logging":17,"../utilities/TypeValidators":25,"./InvokerOptions":7,"./PickerMode":8}],10:[function(_dereq_,module,exports){
var CallbackInvoker = _dereq_('../utilities/CallbackInvoker'), Constants = _dereq_('../Constants'), DomUtilities = _dereq_('../utilities/DomUtilities'), ErrorHandler = _dereq_('../utilities/ErrorHandler'), InvokerOptions = _dereq_('./InvokerOptions'), Logging = _dereq_('../utilities/Logging'), PickerMode = _dereq_('./PickerMode'), StringUtilities = _dereq_('../utilities/StringUtilities'), TypeValidators = _dereq_('../utilities/TypeValidators'), UploadType = _dereq_('./UploadType'), UrlUtilities = _dereq_('../utilities/UrlUtilities');
var FORM_UPLOAD_SIZE_LIMIT = 104857600;
var FORM_UPLOAD_SIZE_LIMIT_STRING = '100 MB';
var SaverOptions = function (_super) {
        __extends(SaverOptions, _super);
        function SaverOptions(options) {
            _super.call(this, options);
            this.invalidFile = false;
            if (this.expectGlobalFunction) {
                this.successName = options.success;
                this.progressName = options.progress;
            }
            var successCallback = TypeValidators.validateCallback(options.success, false, this.expectGlobalFunction);
            this.success = function () {
                Logging.logMessage('saver operation succeeded');
                CallbackInvoker.invokeAppCallback(successCallback, true);
            };
            var progressCallback = TypeValidators.validateCallback(options.progress, true, this.expectGlobalFunction);
            this.progress = function (percentage) {
                Logging.logMessage(StringUtilities.format('upload progress: {0}%', percentage));
                CallbackInvoker.invokeAppCallback(progressCallback, false, percentage);
            };
            this._setFileInfo(options);
        }
        SaverOptions.prototype.serializeState = function () {
            return {
                optionsMode: PickerMode[PickerMode.save],
                options: {
                    success: this.successName,
                    progress: this.progressName,
                    cancel: this.cancelName,
                    error: this.errorName,
                    file: this.file,
                    fileName: this.fileName,
                    openInNewWindow: this.openInNewWindow
                }
            };
        };
        SaverOptions.prototype._setFileInfo = function (options) {
            this.file = TypeValidators.validateType(options.file, Constants.TYPE_STRING);
            var fileName = TypeValidators.validateType(options.fileName, Constants.TYPE_STRING, true, null);
            if (UrlUtilities.isPathFullUrl(this.file)) {
                this.uploadType = UploadType.url;
                this.fileName = fileName || UrlUtilities.getFileNameFromUrl(this.file);
                if (!this.fileName) {
                    ErrorHandler.throwError('must supply a file name or a URL that ends with a file name');
                }
            } else if (UrlUtilities.isPathDataUrl(this.file)) {
                this.uploadType = UploadType.dataUrl;
                this.fileName = fileName;
                if (!this.fileName) {
                    ErrorHandler.throwError('must supply a file name for data URL uploads');
                }
            } else {
                this.uploadType = UploadType.form;
                var fileInputElement = DomUtilities.getElementById(this.file);
                if (fileInputElement instanceof HTMLInputElement) {
                    if (fileInputElement.type !== 'file') {
                        ErrorHandler.throwError('input elemenet must be of type \'file\'');
                    }
                    if (!fileInputElement.value) {
                        this.error({
                            errorCode: 0,
                            message: 'user has not supplied a file to upload'
                        });
                        this.invalidFile = true;
                        return;
                    }
                    var files = fileInputElement.files;
                    if (!files || !window['FileReader']) {
                        ErrorHandler.throwError('browser does not support Files API');
                    }
                    if (files.length !== 1) {
                        ErrorHandler.throwError('can not upload more than one file at a time');
                    }
                    var fileInput = files[0];
                    if (!fileInput) {
                        ErrorHandler.throwError('missing file input');
                    }
                    if (fileInput.size > FORM_UPLOAD_SIZE_LIMIT) {
                        this.error({
                            errorCode: 1,
                            message: 'the user has selected a file larger than ' + FORM_UPLOAD_SIZE_LIMIT_STRING
                        });
                        this.invalidFile = true;
                        return;
                    }
                    this.fileName = fileName || fileInput.name;
                    this.fileInput = fileInput;
                } else {
                    ErrorHandler.throwError('element was not an instance of an HTMLInputElement');
                }
            }
        };
        return SaverOptions;
    }(InvokerOptions);
module.exports = SaverOptions;
},{"../Constants":1,"../utilities/CallbackInvoker":13,"../utilities/DomUtilities":14,"../utilities/ErrorHandler":15,"../utilities/Logging":17,"../utilities/StringUtilities":24,"../utilities/TypeValidators":25,"../utilities/UrlUtilities":26,"./InvokerOptions":7,"./PickerMode":8,"./UploadType":11}],11:[function(_dereq_,module,exports){
var UploadType;
(function (UploadType) {
    UploadType[UploadType['dataUrl'] = 0] = 'dataUrl';
    UploadType[UploadType['form'] = 1] = 'form';
    UploadType[UploadType['url'] = 2] = 'url';
}(UploadType || (UploadType = {})));
module.exports = UploadType;
},{}],12:[function(_dereq_,module,exports){
var Logging = _dereq_('./Logging'), ObjectUtilities = _dereq_('./ObjectUtilities'), OneDriveState = _dereq_('../OneDriveState'), UrlUtilities = _dereq_('./UrlUtilities');
var AccountChooser = function () {
        function AccountChooser() {
        }
        AccountChooser.buildAccountChooserUrlForPicker = function (options) {
            var sdkState = ObjectUtilities.serializeJSON(options.serializeState());
            return AccountChooser._buildAccountChooserUrl('read', options.multiSelect ? 'multi' : 'single', 'file', sdkState, options.linkType);
        };
        AccountChooser.buildAccountChooserUrlForSaver = function (options) {
            var sdkState = ObjectUtilities.serializeJSON(options.serializeState());
            return AccountChooser._buildAccountChooserUrl('readwrite', 'single', 'folder', sdkState);
        };
        AccountChooser._buildAccountChooserUrl = function (access, selectionMode, viewType, sdkState, linkType) {
            var queryParameters = {};
            var aadClientId = OneDriveState.clientIds.aadClientId;
            if (aadClientId) {
                queryParameters['aad_client_id'] = aadClientId;
            }
            var msaClientId = OneDriveState.clientIds.msaClientId;
            if (msaClientId) {
                queryParameters['msa_client_id'] = msaClientId;
            }
            if (linkType) {
                queryParameters['link_type'] = linkType;
            }
            queryParameters['jssdk_state'] = sdkState;
            queryParameters['ru'] = UrlUtilities.trimUrlQuery(window.location.href);
            queryParameters['access'] = access;
            queryParameters['selection_mode'] = selectionMode;
            queryParameters['view_type'] = viewType;
            var fullUrl = UrlUtilities.appendQueryStrings('https://onedrive.' + OneDriveState.getODCHost() + '/picker/accountchooser', queryParameters);
            Logging.logMessage('invoking account chooser: ' + fullUrl);
            return fullUrl;
        };
        return AccountChooser;
    }();
module.exports = AccountChooser;
},{"../OneDriveState":4,"./Logging":17,"./ObjectUtilities":18,"./UrlUtilities":26}],13:[function(_dereq_,module,exports){
var Constants = _dereq_('../Constants'), OneDriveState = _dereq_('../OneDriveState');
var CallbackInvoker = function () {
        function CallbackInvoker() {
        }
        CallbackInvoker.invokeAppCallback = function (callback, isFinalCallback) {
            var args = [];
            for (var _i = 2; _i < arguments.length; _i++) {
                args[_i - 2] = arguments[_i];
            }
            if (isFinalCallback) {
                OneDriveState.clearState();
            }
            if (typeof callback === Constants.TYPE_FUNCTION) {
                callback.apply(null, args);
            }
        };
        CallbackInvoker.invokeCallbackAsynchronous = function (callback) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            window.setTimeout(function () {
                callback.apply(null, args);
            }, 0);
        };
        return CallbackInvoker;
    }();
module.exports = CallbackInvoker;
},{"../Constants":1,"../OneDriveState":4}],14:[function(_dereq_,module,exports){
var ErrorHandler = _dereq_('./ErrorHandler'), Logging = _dereq_('./Logging'), OneDriveState = _dereq_('../OneDriveState'), StringUtilities = _dereq_('./StringUtilities');
var DOM_CLIENT_ID = 'client-id';
var DOM_DEBUG = 'debug';
var DOM_LOGGING_ID = 'enable-logging';
var DOM_SDK_ID = 'onedrive-js';
var MSA_APPID_PATTERN = new RegExp('^([a-fA-F0-9]){16}$');
var AAD_APPID_PATTERN = new RegExp('^[a-fA-F\\d]{8}-([a-fA-F\\d]{4}-){3}[a-fA-F\\d]{12}$');
var DomUtilities = function () {
        function DomUtilities() {
        }
        DomUtilities.getScriptInput = function () {
            var element = DomUtilities.getElementById(DOM_SDK_ID);
            if (element) {
                var enableLogging = element.getAttribute(DOM_LOGGING_ID);
                if (enableLogging === 'true') {
                    Logging.loggingEnabled = true;
                }
                var debugMode = element.getAttribute(DOM_DEBUG);
                if (debugMode === 'true') {
                    OneDriveState.debug = true;
                }
                var rawClientIds = element.getAttribute(DOM_CLIENT_ID);
                if (!rawClientIds) {
                    ErrorHandler.throwError(StringUtilities.format('SDK script tag missing \'{0}\' attribute', DOM_CLIENT_ID));
                }
                var splitClientIds = rawClientIds.split(',').map(function (str) {
                        return str.trim();
                    });
                if (splitClientIds.length < 1 || splitClientIds.length > 2) {
                    ErrorHandler.throwError('expected 1 or 2 client ids');
                }
                var clientIds = {};
                for (var i = 0; i < splitClientIds.length; i++) {
                    var clientId = splitClientIds[i];
                    if (MSA_APPID_PATTERN.test(clientId)) {
                        Logging.logMessage('parsed MSA client id: ' + clientId);
                        clientIds.msaClientId = clientId;
                    } else if (AAD_APPID_PATTERN.test(clientId)) {
                        Logging.logMessage('parsed AAD client id: ' + clientId);
                        clientIds.aadClientId = clientId;
                    } else {
                        ErrorHandler.throwError(StringUtilities.format('invalid format for client id \'{0}\' - MSA: 16 characters (HEX), AAD: 32 characters (HEX) GUID', clientId));
                    }
                }
                OneDriveState.clientIds = clientIds;
            } else {
                ErrorHandler.throwError(StringUtilities.format('SDK script tag missing \'{0}\' id', DOM_SDK_ID));
            }
        };
        DomUtilities.getElementById = function (id) {
            return document.getElementById(id);
        };
        DomUtilities.onDocumentReady = function (callback) {
            if (document.readyState === 'interactive' || document.readyState === 'complete') {
                callback();
            } else {
                document.addEventListener('DOMContentLoaded', callback, false);
            }
        };
        return DomUtilities;
    }();
module.exports = DomUtilities;
},{"../OneDriveState":4,"./ErrorHandler":15,"./Logging":17,"./StringUtilities":24}],15:[function(_dereq_,module,exports){
var Logging = _dereq_('./Logging');
var ERROR_PREFIX = '[OneDriveSDK Error] ';
var ErrorHandler = function () {
        function ErrorHandler() {
        }
        ErrorHandler.registerErrorObserver = function (callback) {
            ErrorHandler._errorObservers.push(callback);
        };
        ErrorHandler.throwError = function (message) {
            var callbacks = ErrorHandler._errorObservers;
            for (var index in callbacks) {
                try {
                    callbacks[index]();
                } catch (error) {
                    Logging.logError('exception thrown invoking error observer', error);
                }
            }
            throw new Error(ERROR_PREFIX + message);
        };
        ErrorHandler._errorObservers = [];
        return ErrorHandler;
    }();
module.exports = ErrorHandler;
},{"./Logging":17}],16:[function(_dereq_,module,exports){
var Constants = _dereq_('../Constants'), Logging = _dereq_('./Logging'), ObjectUtilities = _dereq_('./ObjectUtilities'), OneDriveState = _dereq_('../OneDriveState'), StringUtilities = _dereq_('./StringUtilities'), UrlUtilities = _dereq_('./UrlUtilities'), XHR = _dereq_('./XHR');
var BATCH_SIZE = 10;
var FilesV2Wrapper = function () {
        function FilesV2Wrapper() {
        }
        FilesV2Wrapper.callFilesV2Open = function (response, getWebLinks, success, error) {
            var apiEndpointUrl = response.apiEndpointUrl;
            var apiEndpoint = response.apiEndpoint;
            var accessToken = response.accessToken;
            var clientId = OneDriveState.clientIds.aadClientId;
            var itemIds = response.itemIds;
            var queryParameters = {};
            queryParameters['expand'] = 'thumbnails';
            queryParameters['select'] = 'id,content.downloadUrl,name,size';
            var requestHeaders = {};
            requestHeaders['Authorization'] = 'bearer ' + accessToken;
            requestHeaders['Cache-Control'] = 'no-cache, no-store, must-revalidate';
            var successObjects = [];
            var errorCount = 0;
            var totalResponses = 0;
            var numItems = itemIds.length;
            var invokeCallbacks;
            var runBatch = function (batchStart, batchEnd) {
                Logging.logMessage(StringUtilities.format('running batch for items \'{0}\' - \'{1}\'', batchStart + 1, batchEnd + 1));
                for (var i = batchStart; i < batchEnd; i++) {
                    var itemId = itemIds[i];
                    var url = UrlUtilities.appendToPath(apiEndpointUrl, 'drive/items/' + itemId);
                    var xhr = new XHR({
                            url: UrlUtilities.appendQueryStrings(url, queryParameters),
                            clientId: clientId,
                            method: Constants.HTTP_GET,
                            apiEndpoint: apiEndpoint,
                            headers: requestHeaders
                        });
                    Logging.logMessage('performing GET on item with id: ' + itemId);
                    xhr.start(function (xhr, statusCode, url) {
                        var successResponse = ObjectUtilities.deserializeJSON(xhr.responseText);
                        if (getWebLinks) {
                            var generateSharingLink = function (successObjects, successResponse, requestUrl) {
                                var postXhr = new XHR({
                                        url: requestUrl,
                                        clientId: OneDriveState.clientIds.aadClientId,
                                        method: Constants.HTTP_POST,
                                        apiEndpoint: apiEndpoint,
                                        headers: requestHeaders,
                                        json: '{"type" : "view"}'
                                    });
                                postXhr.start(function (postXhr, statusCode, url) {
                                    Logging.logMessage(StringUtilities.format('POST createLink succeeded via path {0}', requestUrl));
                                    var successPostResponse = ObjectUtilities.deserializeJSON(postXhr.responseText);
                                    successResponse.webUrl = successPostResponse.link.webUrl;
                                    successResponse.accessToken = accessToken;
                                    successObjects.push(successResponse);
                                    invokeCallbacks();
                                }, function (xhr, statusCode, timeout) {
                                    Logging.logMessage(StringUtilities.format('POST createLink failed via path {0}', requestUrl));
                                    errorCount++;
                                    invokeCallbacks();
                                });
                            };
                            generateSharingLink(successObjects, successResponse, UrlUtilities.trimUrlQuery(url) + '/action.createLink');
                        } else {
                            successObjects.push(successResponse);
                            invokeCallbacks();
                        }
                    }, function (xhr, statusCode, timeout) {
                        errorCount++;
                        invokeCallbacks();
                    });
                }
            };
            invokeCallbacks = function () {
                if (++totalResponses === numItems) {
                    if (successObjects.length) {
                        Logging.logMessage(StringUtilities.format('GET metadata succeeded for \'{0}\' items', successObjects.length));
                        success(successObjects);
                    }
                    if (errorCount) {
                        Logging.logMessage(StringUtilities.format('GET metadata failed for \'{0}\' items', errorCount));
                        error(errorCount);
                    }
                } else if (totalResponses % BATCH_SIZE === 0) {
                    runBatch(totalResponses, Math.min(numItems, totalResponses + BATCH_SIZE));
                }
            };
            runBatch(0, Math.min(numItems, BATCH_SIZE));
        };
        return FilesV2Wrapper;
    }();
module.exports = FilesV2Wrapper;
},{"../Constants":1,"../OneDriveState":4,"./Logging":17,"./ObjectUtilities":18,"./StringUtilities":24,"./UrlUtilities":26,"./XHR":29}],17:[function(_dereq_,module,exports){
var ENABLE_LOGGING = 'onedrive_enable_logging';
var LOG_PREFIX = '[OneDriveSDK] ';
var Logging = function () {
        function Logging() {
        }
        Logging.logError = function (message) {
            var objects = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                objects[_i - 1] = arguments[_i];
            }
            Logging._log(message, true, objects);
        };
        Logging.logMessage = function (message) {
            Logging._log(message, false);
        };
        Logging._log = function (message, isError) {
            var objects = [];
            for (var _i = 2; _i < arguments.length; _i++) {
                objects[_i - 2] = arguments[_i];
            }
            if (isError || Logging.loggingEnabled || window[ENABLE_LOGGING]) {
                console.log(LOG_PREFIX + message, objects);
            }
        };
        Logging.loggingEnabled = false;
        return Logging;
    }();
module.exports = Logging;
},{}],18:[function(_dereq_,module,exports){
var Constants = _dereq_('../Constants'), Logging = _dereq_('./Logging');
var ObjectUtilities = function () {
        function ObjectUtilities() {
        }
        ObjectUtilities.shallowClone = function (object) {
            if (typeof object !== Constants.TYPE_OBJECT || !object) {
                return null;
            }
            var clonedObject = {};
            for (var key in object) {
                if (object.hasOwnProperty(key)) {
                    clonedObject[key] = object[key];
                }
            }
            return clonedObject;
        };
        ObjectUtilities.deserializeJSON = function (text) {
            var deserializedObject = null;
            try {
                deserializedObject = JSON.parse(text);
            } catch (error) {
                Logging.logError('deserialization error' + error);
            }
            if (typeof deserializedObject !== Constants.TYPE_OBJECT || deserializedObject === null) {
                deserializedObject = {};
            }
            return deserializedObject;
        };
        ObjectUtilities.serializeJSON = function (value) {
            return JSON.stringify(value);
        };
        return ObjectUtilities;
    }();
module.exports = ObjectUtilities;
},{"../Constants":1,"./Logging":17}],19:[function(_dereq_,module,exports){
var AccountChooser = _dereq_('./AccountChooser'), Constants = _dereq_('../Constants'), ErrorHandler = _dereq_('./ErrorHandler'), ErrorType = _dereq_('../models/ErrorType'), FilesV2Wrapper = _dereq_('./FilesV2Wrapper'), Logging = _dereq_('./Logging'), ObjectUtilities = _dereq_('./ObjectUtilities'), Popup = _dereq_('./Popup'), PickerOptions = _dereq_('../models/PickerOptions'), RedirectUtilities = _dereq_('./RedirectUtilities'), StringUtilities = _dereq_('./StringUtilities'), VroomWrapper = _dereq_('./VroomWrapper');
var VROOM_THUMBNAIL_SIZES = [
        'large',
        'medium',
        'small'
    ];
var Picker = function () {
        function Picker(options) {
            var clonedOptions = ObjectUtilities.shallowClone(options);
            this._pickerOptions = new PickerOptions(clonedOptions);
        }
        Picker.prototype.launchPicker = function () {
            var _this = this;
            var pickerOptions = this._pickerOptions;
            var url = AccountChooser.buildAccountChooserUrlForPicker(pickerOptions);
            var windowState = pickerOptions.serializeState();
            if (pickerOptions.openInNewWindow) {
                var popup = new Popup(url, function (response) {
                        _this.handlePickerSuccess(response);
                    }, function (response) {
                        _this.handlePickerError(response);
                    });
                if (!popup.openPopup()) {
                    pickerOptions.error(Constants.ERROR_POPUP_OPEN);
                }
            } else {
                RedirectUtilities.redirect(url, windowState);
            }
        };
        Picker.prototype.handlePickerSuccess = function (pickerResponse) {
            var pickerType = pickerResponse.pickerType;
            switch (pickerType) {
            case Constants.STATE_MSA_PICKER:
                this._handleMSAOpenResponse(pickerResponse);
                break;
            case Constants.STATE_AAD_PICKER:
                this._handleAADOpenResponse(pickerResponse);
                break;
            default:
                ErrorHandler.throwError('invalid value for picker type: ' + pickerType);
            }
        };
        Picker.prototype.handlePickerError = function (errorResponse) {
            if (errorResponse.error === Constants.ERROR_ACCESS_DENIED) {
                this._pickerOptions.cancel();
            } else {
                this._pickerOptions.error({
                    errorCode: ErrorType.unknown,
                    message: 'something went wrong: ' + errorResponse.error
                });
            }
        };
        Picker.prototype._handleMSAOpenResponse = function (pickerResponse) {
            var _this = this;
            var options = this._pickerOptions;
            VroomWrapper.callVroomOpen(pickerResponse, options.getWebLinks, function (apiResponse) {
                _this._handleSuccessResponse({
                    webUrl: apiResponse.webUrl,
                    files: options.getWebLinks ? apiResponse.children && apiResponse.children.length > 0 ? apiResponse.children : [apiResponse] : apiResponse.value
                }, true);
            }, function () {
                options.error(Constants.ERROR_WEB_REQUEST);
            });
        };
        Picker.prototype._handleAADOpenResponse = function (pickerResponse) {
            var _this = this;
            var options = this._pickerOptions;
            FilesV2Wrapper.callFilesV2Open(pickerResponse, options.getWebLinks, function (apiResponse) {
                _this._handleSuccessResponse({
                    webUrl: null,
                    files: apiResponse
                });
            }, function (errorCount) {
                options.error({
                    errorCode: ErrorType.webRequestFailure,
                    message: StringUtilities.format('\'{0}\' web request(s) failed, see console log for details', errorCount)
                });
            });
        };
        Picker.prototype._handleSuccessResponse = function (response, isMSA) {
            var options = this._pickerOptions;
            var files = {
                    link: options.getWebLinks ? response.webUrl : null,
                    values: []
                };
            var pickerFiles = response.files;
            if (!pickerFiles || !pickerFiles.length) {
                options.error({
                    errorCode: ErrorType.badResponse,
                    message: 'no files returned'
                });
            }
            Logging.logMessage(StringUtilities.format('returning \'{0}\' files picked', pickerFiles.length));
            for (var i = 0; i < pickerFiles.length; i++) {
                var pickerFile = pickerFiles[i];
                var fileLink = options.getWebLinks ? pickerFile.webUrl : pickerFile['@content.downloadUrl'];
                var file = {
                        fileName: pickerFile.name,
                        link: fileLink,
                        linkType: options.linkType,
                        size: pickerFile.size,
                        accessToken: pickerFile.accessToken
                    };
                if (isMSA) {
                    var thumbnails = [];
                    var fileThumbnails = pickerFile.thumbnails && pickerFile.thumbnails[0];
                    if (fileThumbnails) {
                        for (var j = 0; j < VROOM_THUMBNAIL_SIZES.length; j++) {
                            thumbnails.push(fileThumbnails[VROOM_THUMBNAIL_SIZES[j]].url);
                        }
                    }
                    file.thumbnails = thumbnails;
                }
                files.values.push(file);
            }
            options.success(files);
        };
        return Picker;
    }();
module.exports = Picker;
},{"../Constants":1,"../models/ErrorType":6,"../models/PickerOptions":9,"./AccountChooser":12,"./ErrorHandler":15,"./FilesV2Wrapper":16,"./Logging":17,"./ObjectUtilities":18,"./Popup":20,"./RedirectUtilities":21,"./StringUtilities":24,"./VroomWrapper":27}],20:[function(_dereq_,module,exports){
var CallbackInvoker = _dereq_('./CallbackInvoker'), Constants = _dereq_('../Constants'), Logging = _dereq_('./Logging'), ResponseParser = _dereq_('./ResponseParser');
var POPUP_WIDTH = 800;
var POPUP_HEIGHT = 650;
var POPUP_PINGER_INTERVAL = 500;
var Popup = function () {
        function Popup(url, successCallback, errorCallabck) {
            this._messageCallbackInvoked = false;
            this._url = url;
            this._successCallback = successCallback;
            this._failureCallback = errorCallabck;
        }
        Popup.canReceiveMessage = function (event) {
            return event.origin === window.location.origin;
        };
        Popup._createPopupFeatures = function () {
            var left = window.screenX + Math.max(window.outerWidth - POPUP_WIDTH, 0) / 2;
            var top = window.screenY + Math.max(window.outerHeight - POPUP_HEIGHT, 0) / 2;
            var features = [
                    'width=' + POPUP_WIDTH,
                    'height=' + POPUP_HEIGHT,
                    'top=' + top,
                    'left=' + left,
                    'status=no',
                    'resizable=yes',
                    'toolbar=no',
                    'menubar=no',
                    'scrollbars=yes'
                ];
            return features.join(',');
        };
        Popup.prototype.openPopup = function () {
            if (Popup._currentPopup && Popup._currentPopup._isPopupOpen()) {
                return false;
            }
            if (!window.onedriveReceiveMessage) {
                window.onedriveReceiveMessage = function (data) {
                    var currentPopup = Popup._currentPopup;
                    if (currentPopup && currentPopup._isPopupOpen()) {
                        Popup._currentPopup = null;
                        var response = ResponseParser.parsePickerResponse(data);
                        currentPopup._messageCallbackInvoked = true;
                        if (response.error === undefined) {
                            CallbackInvoker.invokeCallbackAsynchronous(currentPopup._successCallback, response);
                        } else {
                            CallbackInvoker.invokeCallbackAsynchronous(currentPopup._failureCallback, response);
                        }
                    }
                };
            }
            this._popup = window.open(this._url, '_blank', Popup._createPopupFeatures());
            this._popup.focus();
            this._createPopupPinger();
            Popup._currentPopup = this;
            return true;
        };
        Popup.prototype._createPopupPinger = function () {
            var _this = this;
            var interval = window.setInterval(function () {
                    if (_this._isPopupOpen()) {
                        _this._popup.postMessage('ping', '*');
                    } else {
                        window.clearInterval(interval);
                        Popup._currentPopup = null;
                        if (!_this._messageCallbackInvoked) {
                            Logging.logMessage('closed callback');
                            _this._failureCallback({ error: Constants.ERROR_ACCESS_DENIED });
                        }
                    }
                }, POPUP_PINGER_INTERVAL);
        };
        Popup.prototype._isPopupOpen = function () {
            return this._popup !== null && !this._popup.closed;
        };
        return Popup;
    }();
module.exports = Popup;
},{"../Constants":1,"./CallbackInvoker":13,"./Logging":17,"./ResponseParser":22}],21:[function(_dereq_,module,exports){
var Constants = _dereq_('../Constants'), DomUtilities = _dereq_('./DomUtilities'), ErrorHandler = _dereq_('./ErrorHandler'), Logging = _dereq_('./Logging'), ObjectUtilities = _dereq_('./ObjectUtilities'), OneDriveState = _dereq_('../OneDriveState'), StringUtilities = _dereq_('./StringUtilities'), TypeValidators = _dereq_('./TypeValidators'), UrlUtilities = _dereq_('./UrlUtilities'), WindowState = _dereq_('./WindowState'), XHR = _dereq_('./XHR');
var AAD_LOGIN_URL = 'https://login.microsoftonline.com/common/oauth2/authorize';
var DISCOVERY_URL = 'https://onedrive.live.com/picker/businessurldiscovery';
var RedirectUtilities = function () {
        function RedirectUtilities() {
        }
        RedirectUtilities.redirect = function (url, values, windowState) {
            if (values === void 0) {
                values = null;
            }
            if (windowState === void 0) {
                windowState = null;
            }
            if (values) {
                WindowState.setWindowState(values, windowState);
            }
            UrlUtilities.validateUrlProtocol(url);
            window.location.replace(url);
        };
        RedirectUtilities.handleRedirect = function () {
            var queryParameters = UrlUtilities.readCurrentUrlParameters();
            var serializedState = WindowState.getWindowState();
            var state = queryParameters[Constants.PARAM_STATE] || serializedState[Constants.PARAM_STATE];
            if (!state && queryParameters[Constants.PARAM_ERROR] === Constants.ERROR_ACCESS_DENIED) {
                queryParameters[Constants.PARAM_STATE] = Constants.STATE_MSA_PICKER;
            } else if (state === Constants.STATE_AAD_PICKER) {
                queryParameters[Constants.PARAM_STATE] = Constants.STATE_AAD_PICKER;
            }
            var redirectState = queryParameters[Constants.PARAM_STATE];
            if (!redirectState) {
                return null;
            }
            Logging.logMessage('current state: ' + redirectState);
            var options = serializedState['options'];
            if (!options) {
                ErrorHandler.throwError('missing options from serialized state');
            }
            var inPopupFlow = TypeValidators.validateType(options.openInNewWindow, Constants.TYPE_BOOLEAN);
            if (inPopupFlow) {
                RedirectUtilities._displayOverlay();
            }
            switch (redirectState) {
            case Constants.STATE_DISCOVERY:
                RedirectUtilities._handleDiscoverRedirect(queryParameters, inPopupFlow);
                break;
            case Constants.STATE_AAD_LOGIN:
                RedirectUtilities._handleAADTenantLoginRedirect(queryParameters, inPopupFlow);
                break;
            case Constants.STATE_MSA_PICKER:
            case Constants.STATE_AAD_PICKER:
                var pickerResponse = {
                        windowState: serializedState,
                        queryParameters: queryParameters
                    };
                Logging.logMessage('sending invoker response');
                if (inPopupFlow) {
                    RedirectUtilities._sendResponse(pickerResponse);
                } else {
                    return pickerResponse;
                }
                break;
            default:
                ErrorHandler.throwError('invalid value for redirect state: ' + redirectState);
            }
            return null;
        };
        RedirectUtilities._handleDiscoverRedirect = function (queryParameters, inPopupFlow) {
            if (!inPopupFlow) {
                RedirectUtilities._displayOverlay();
            }
            var accessToken = queryParameters[Constants.PARAM_ACCESS_TOKEN];
            if (!accessToken) {
                RedirectUtilities._handleError('missing access token', inPopupFlow);
            }
            var xhr = new XHR({
                    url: UrlUtilities.appendQueryString(DISCOVERY_URL, Constants.PARAM_ACCESS_TOKEN, accessToken),
                    method: Constants.HTTP_GET
                });
            xhr.start(function (xhr, statusCode) {
                var response = ObjectUtilities.deserializeJSON(xhr.responseText);
                if (!response.value || response.value.length < 2) {
                    RedirectUtilities._handleError('invalid response from discovery service', inPopupFlow);
                }
                var responseValues = response.value[1];
                if (!responseValues) {
                    RedirectUtilities._handleError('received discovery response missing tenant infromation', inPopupFlow);
                }
                var tenantUrl = responseValues.serviceResourceId;
                var apiEndpoint = responseValues.serviceEndpointUri;
                var queryParameters = {
                        client_id: OneDriveState.clientIds.aadClientId,
                        resource: tenantUrl,
                        response_type: 'token',
                        redirect_uri: UrlUtilities.trimUrlQuery(window.location.href),
                        state: Constants.STATE_AAD_LOGIN
                    };
                var stateValue = {
                        discovery: {
                            tenantUrl: tenantUrl,
                            apiEndpoint: apiEndpoint
                        }
                    };
                RedirectUtilities.redirect(UrlUtilities.appendQueryStrings(AAD_LOGIN_URL, queryParameters), stateValue);
            }, function (xhr, statusCode, timeout) {
                RedirectUtilities._handleError(StringUtilities.format('discovery request failed, status code: \'{0}\', response text: \'{1}\'', XHR.statusCodeToString(statusCode), xhr.responseText), inPopupFlow);
            });
        };
        RedirectUtilities._handleAADTenantLoginRedirect = function (queryParameters, inPopupFlow) {
            if (!inPopupFlow) {
                RedirectUtilities._displayOverlay();
            }
            var accessToken = queryParameters[Constants.PARAM_ACCESS_TOKEN];
            if (!accessToken) {
                RedirectUtilities._handleError('missing api access token', inPopupFlow);
            }
            var stateValues = {
                    aadAccessToken: accessToken,
                    state: Constants.STATE_AAD_PICKER
                };
            var windowState = WindowState.getWindowState();
            var tenantUrl = windowState.discovery.tenantUrl;
            if (!tenantUrl) {
                RedirectUtilities._handleError('missing tenant url', inPopupFlow);
            }
            RedirectUtilities.redirect(UrlUtilities.appendToPath(tenantUrl, 'MySiteRedirect.aspx?MySiteRedirect=AllDocuments&OneDrivePicker=1'), stateValues, windowState);
        };
        RedirectUtilities._sendResponse = function (response) {
            var parentWindow = window.opener;
            if (parentWindow.onedriveReceiveMessage) {
                parentWindow.onedriveReceiveMessage(response);
            } else {
                Logging.logError('error in window\'s opener, pop up will close.');
                ErrorHandler.throwError('SDK message receiver is undefined.');
            }
            window.close();
        };
        RedirectUtilities._handleError = function (error, inPopupFlow) {
            var errorQueryParametere = {};
            errorQueryParametere[Constants.PARAM_ERROR] = error;
            if (inPopupFlow) {
                RedirectUtilities._sendResponse({ queryParameters: errorQueryParametere });
            } else {
                Logging.logMessage('error in picker flow, redirecting back to app');
                errorQueryParametere[Constants.PARAM_STATE] = Constants.STATE_AAD_PICKER;
                var redirectUrl = UrlUtilities.trimUrlQuery(window.location.href);
                RedirectUtilities.redirect(UrlUtilities.appendQueryStrings(redirectUrl, errorQueryParametere));
            }
        };
        RedirectUtilities._displayOverlay = function () {
            var overlay = document.createElement('div');
            var overlayStyle = [
                    'position: fixed',
                    'width: 100%',
                    'height: 100%',
                    'top: 0px',
                    'left: 0px',
                    'background-color: white',
                    'opacity: 1',
                    'z-index: 10000'
                ];
            overlay.id = 'od-overlay';
            overlay.style.cssText = overlayStyle.join(';');
            var spinner = document.createElement('img');
            var spinnerStyle = [
                    'position: absolute',
                    'top: calc(50% - 40px)',
                    'left: calc(50% - 40px)'
                ];
            spinner.id = 'od-spinner';
            spinner.src = 'https://p.sfx.ms/common/spinner_grey_40_transparent.gif';
            spinner.style.cssText = spinnerStyle.join(';');
            overlay.appendChild(spinner);
            var hiddenStyle = document.createElement('style');
            hiddenStyle.type = 'text/css';
            hiddenStyle.innerHTML = 'body { visibility: hidden !important; }';
            document.head.appendChild(hiddenStyle);
            DomUtilities.onDocumentReady(function () {
                var documentBody = document.body;
                if (documentBody !== null) {
                    documentBody.insertBefore(overlay, documentBody.firstChild);
                } else {
                    document.createElement('body').appendChild(overlay);
                }
                document.head.removeChild(hiddenStyle);
            });
        };
        return RedirectUtilities;
    }();
module.exports = RedirectUtilities;
},{"../Constants":1,"../OneDriveState":4,"./DomUtilities":14,"./ErrorHandler":15,"./Logging":17,"./ObjectUtilities":18,"./StringUtilities":24,"./TypeValidators":25,"./UrlUtilities":26,"./WindowState":28,"./XHR":29}],22:[function(_dereq_,module,exports){
var ApiEndpoint = _dereq_('../models/ApiEndpoint'), Constants = _dereq_('../Constants'), ErrorHandler = _dereq_('./ErrorHandler'), Logging = _dereq_('./Logging'), OneDriveState = _dereq_('../OneDriveState');
var CID_PADDING = '0000000000000000';
var CID_PADDING_LENGTH = CID_PADDING.length;
var MSA_SCOPE_RESPONSE_PATTERN = new RegExp('^\\w+\\.\\w+:\\w+[\\|\\w+]+:([\\w]+\\!\\d+)(?:\\!(.+))*$');
var ResponseParser = function () {
        function ResponseParser() {
        }
        ResponseParser.parsePickerResponse = function (response) {
            Logging.logMessage('parsing picker response');
            var serializedState = response.windowState;
            if (!serializedState) {
                ErrorHandler.throwError('missing windowState from picker response');
            }
            var queryParameters = response.queryParameters;
            if (!queryParameters) {
                ErrorHandler.throwError('missing queryParameters from picker response');
            }
            var responseError = queryParameters[Constants.PARAM_ERROR];
            if (responseError) {
                return { error: responseError };
            }
            var pickerType = queryParameters[Constants.PARAM_STATE];
            var result = { pickerType: pickerType };
            switch (pickerType) {
            case Constants.STATE_MSA_PICKER:
                result.apiEndpoint = ApiEndpoint.vroom;
                ResponseParser._parseMSAResponse(result, queryParameters);
                break;
            case Constants.STATE_AAD_PICKER:
                result.apiEndpoint = ApiEndpoint.filesV2;
                ResponseParser._parseAADResponse(result, queryParameters, serializedState);
                break;
            default:
                ErrorHandler.throwError('invalid value for picker type: ' + pickerType);
            }
            if (!result.accessToken) {
                ErrorHandler.throwError('missing access token');
            }
            if (!result.apiEndpointUrl) {
                ErrorHandler.throwError('missing API endpoint URL');
            }
            return result;
        };
        ResponseParser._parseMSAResponse = function (result, queryParameters) {
            result.accessToken = queryParameters[Constants.PARAM_ACCESS_TOKEN];
            result.apiEndpointUrl = OneDriveState.debug ? Constants.VROOM_INT_URL : Constants.VROOM_URL;
            var responseScope = queryParameters['scope'];
            if (!responseScope) {
                ErrorHandler.throwError('missing \'scope\' paramter from MSA picker response');
            }
            var scopes = responseScope.split(' ');
            var scopeResult;
            for (var i = 0; i < scopes.length && !scopeResult; i++) {
                scopeResult = MSA_SCOPE_RESPONSE_PATTERN.exec(scopes[i]);
            }
            if (!scopeResult) {
                ErrorHandler.throwError('scope was not formatted correctly');
            }
            var rawResult = scopeResult[1].split('_');
            var rawItemId = rawResult[1];
            var splitIndex = rawItemId.indexOf('!');
            var rawItemIdPart1 = rawItemId.substring(0, splitIndex);
            var rawItemIdPart2 = rawItemId.substring(splitIndex);
            var ownerCid = ResponseParser._leftPadCid(rawItemIdPart1);
            var itemId = ownerCid + rawItemIdPart2;
            result.ownerCid = ownerCid;
            result.itemId = itemId;
            result.authKey = scopeResult[2];
        };
        ResponseParser._parseAADResponse = function (result, queryParameters, state) {
            result.accessToken = state.aadAccessToken;
            result.apiEndpointUrl = state.discovery && state.discovery.apiEndpoint;
            var itemIds = queryParameters['item-id'].split(',');
            if (!itemIds.length) {
                ErrorHandler.throwError('missing item id(s)');
            }
            result.itemIds = itemIds;
        };
        ResponseParser._leftPadCid = function (cid) {
            if (cid.length === CID_PADDING_LENGTH) {
                return cid;
            }
            return CID_PADDING.substring(0, CID_PADDING_LENGTH - cid.length) + cid;
        };
        return ResponseParser;
    }();
module.exports = ResponseParser;
},{"../Constants":1,"../OneDriveState":4,"../models/ApiEndpoint":5,"./ErrorHandler":15,"./Logging":17}],23:[function(_dereq_,module,exports){
var AccountChooser = _dereq_('./AccountChooser'), CallbackInvoker = _dereq_('./CallbackInvoker'), Constants = _dereq_('../Constants'), ErrorHandler = _dereq_('./ErrorHandler'), ErrorType = _dereq_('../models/ErrorType'), Logging = _dereq_('./Logging'), ObjectUtilities = _dereq_('./ObjectUtilities'), OneDriveState = _dereq_('../OneDriveState'), Popup = _dereq_('./Popup'), RedirectUtilities = _dereq_('./RedirectUtilities'), SaverOptions = _dereq_('../models/SaverOptions'), StringUtilities = _dereq_('./StringUtilities'), UploadType = _dereq_('../models/UploadType'), UrlUtilities = _dereq_('./UrlUtilities'), VroomWrapper = _dereq_('./VroomWrapper'), XHR = _dereq_('./XHR');
var POLLING_INTERVAL = 1000;
var POLLING_COUNTER = 5;
var Saver = function () {
        function Saver(options) {
            var clonedOptions = ObjectUtilities.shallowClone(options);
            this._saverOptions = new SaverOptions(clonedOptions);
        }
        Saver.prototype.launchSaver = function () {
            var _this = this;
            var saverOptions = this._saverOptions;
            if (saverOptions.invalidFile) {
                return;
            }
            var url = AccountChooser.buildAccountChooserUrlForSaver(saverOptions);
            var windowState = saverOptions.serializeState();
            if (saverOptions.openInNewWindow) {
                var popup = new Popup(url, function (response) {
                        _this.handleSaverSuccess(response);
                    }, function (response) {
                        _this.handleSaverError(response);
                    });
                if (!popup.openPopup()) {
                    saverOptions.error(Constants.ERROR_POPUP_OPEN);
                }
            } else {
                RedirectUtilities.redirect(url, windowState);
            }
        };
        Saver.prototype.handleSaverSuccess = function (saverResponse) {
            var _this = this;
            var pickerType = saverResponse.pickerType;
            switch (pickerType) {
            case Constants.STATE_MSA_PICKER:
                VroomWrapper.callVroomSave(saverResponse, function (apiResponse) {
                    var apiResponseValue = apiResponse.value;
                    if (!apiResponseValue) {
                        ErrorHandler.throwError('empty API response');
                    }
                    var folderId = apiResponseValue[0].id;
                    if (!folderId || apiResponseValue.length !== 1) {
                        ErrorHandler.throwError('incorrect number of folders returned');
                    }
                    _this._executeUpload(saverResponse, folderId);
                }, function () {
                    _this._saverOptions.error(Constants.ERROR_WEB_REQUEST);
                });
                break;
            case Constants.STATE_AAD_PICKER:
                var folderIds = saverResponse.itemIds;
                if (folderIds.length !== 1) {
                    ErrorHandler.throwError('incorrect number of folders returned');
                }
                var folderId = folderIds[0];
                if (!folderId) {
                    folderId = 'root';
                }
                if (OneDriveState.debug && OneDriveDebug) {
                    OneDriveDebug.accessToken = saverResponse.accessToken;
                }
                this._executeUpload(saverResponse, folderId);
                break;
            default:
                ErrorHandler.throwError('invalid value for picker type: ' + pickerType);
            }
        };
        Saver.prototype.handleSaverError = function (errorResponse) {
            if (errorResponse.error === Constants.ERROR_ACCESS_DENIED) {
                this._saverOptions.cancel();
            } else {
                this._saverOptions.error({
                    errorCode: ErrorType.unknown,
                    message: 'something went wrong: ' + errorResponse.error
                });
            }
        };
        Saver.prototype._executeUpload = function (saverResponse, folderId) {
            var uploadType = this._saverOptions.uploadType;
            Logging.logMessage(StringUtilities.format('beginning \'{0}\' upload', UploadType[uploadType]));
            var accessToken = saverResponse.accessToken;
            switch (uploadType) {
            case UploadType.dataUrl:
            case UploadType.url:
                this._executeUrlUpload(saverResponse, folderId, accessToken, uploadType);
                break;
            case UploadType.form:
                this._executeFormUpload(saverResponse, folderId, accessToken);
                break;
            default:
                ErrorHandler.throwError('invalid value for upload type: ' + uploadType);
            }
        };
        Saver.prototype._executeUrlUpload = function (saverResponse, folderId, accessToken, uploadType) {
            var _this = this;
            var options = this._saverOptions;
            if (uploadType === UploadType.url && saverResponse.pickerType === Constants.STATE_AAD_PICKER) {
                options.error({
                    errorCode: ErrorType.unsupportedFeature,
                    message: 'URL upload not supported for AAD'
                });
                return;
            }
            var uploadUrl = UrlUtilities.appendToPath(saverResponse.apiEndpointUrl, 'drive/items/' + folderId + '/children');
            var requestHeaders = {};
            requestHeaders['Prefer'] = 'respond-async';
            requestHeaders['Authorization'] = 'bearer ' + accessToken;
            var body = {
                    '@content.sourceUrl': options.file,
                    'name': options.fileName,
                    'file': {}
                };
            var xhr = new XHR({
                    url: uploadUrl,
                    clientId: OneDriveState.clientIds.msaClientId,
                    method: Constants.HTTP_POST,
                    headers: requestHeaders,
                    json: ObjectUtilities.serializeJSON(body),
                    apiEndpoint: saverResponse.apiEndpoint
                });
            xhr.start(function (xhr, statusCode) {
                if (uploadType === UploadType.dataUrl && (statusCode === 200 || statusCode === 201)) {
                    if (OneDriveState.debug && OneDriveDebug) {
                        OneDriveDebug.itemUrl = ObjectUtilities.deserializeJSON(xhr.responseText)['@odata.id'];
                    }
                    options.success();
                } else if (uploadType === UploadType.url && statusCode === 202) {
                    var location_1 = xhr.getResponseHeader('Location');
                    if (!location_1) {
                        options.error({
                            errorCode: ErrorType.badResponse,
                            message: 'missing \'Location\' header on response'
                        });
                    }
                    _this._beginPolling(location_1, accessToken);
                } else {
                    options.error(Constants.ERROR_WEB_REQUEST);
                }
            }, function (xhr, statusCode, timeout) {
                options.error(Constants.ERROR_WEB_REQUEST);
            });
        };
        Saver.prototype._executeFormUpload = function (saverResponse, folderId, accessToken) {
            var options = this._saverOptions;
            var uploadSource = options.fileInput;
            var reader = null;
            if (window['File'] && uploadSource instanceof window['File']) {
                reader = new FileReader();
            } else {
                ErrorHandler.throwError('file reader not supported');
            }
            reader.onerror = function (event) {
                Logging.logError('failed to read or upload the file', event);
                options.error({
                    errorCode: ErrorType.fileReaderFailure,
                    message: 'failed to read or upload the file, see console log for details'
                });
            };
            reader.onload = function (event) {
                var uploadUrl = UrlUtilities.appendToPath(saverResponse.apiEndpointUrl, 'drive/items/' + folderId + '/children/' + options.fileName + '/content');
                var queryParameters = {};
                queryParameters['@name.conflictBehavior'] = saverResponse.pickerType === Constants.STATE_AAD_PICKER ? 'fail' : 'rename';
                var requestHeaders = {};
                requestHeaders['Authorization'] = 'bearer ' + accessToken;
                var xhr = new XHR({
                        url: UrlUtilities.appendQueryStrings(uploadUrl, queryParameters),
                        clientId: OneDriveState.clientIds.msaClientId,
                        headers: requestHeaders,
                        apiEndpoint: saverResponse.apiEndpoint
                    });
                var data = event.target.result;
                xhr.upload(data, function (xhr, statusCode) {
                    options.success();
                }, function (xhr, statusCode, timeout) {
                    options.error(Constants.ERROR_WEB_REQUEST);
                }, function (xhr, uploadProgress) {
                    options.progress(uploadProgress.progressPercentage);
                });
            };
            reader.readAsArrayBuffer(uploadSource);
        };
        Saver.prototype._beginPolling = function (location, accessToken) {
            Logging.logMessage('polling for URL upload completion');
            var pollingInterval = POLLING_INTERVAL;
            var pollCount = POLLING_COUNTER;
            var xhrOptions = {
                    url: UrlUtilities.appendQueryString(location, Constants.PARAM_ACCESS_TOKEN, accessToken),
                    method: Constants.HTTP_GET
                };
            var options = this._saverOptions;
            var pollForProgress = function () {
                var xhr = new XHR(xhrOptions);
                xhr.start(function (xhr, statusCode) {
                    switch (statusCode) {
                    case 202:
                        var apiResponse = ObjectUtilities.deserializeJSON(xhr.responseText);
                        options.progress(apiResponse['percentageComplete']);
                        if (!pollCount--) {
                            pollingInterval *= 2;
                            pollCount = POLLING_COUNTER;
                        }
                        CallbackInvoker.invokeCallbackAsynchronous(pollForProgress, pollingInterval);
                        break;
                    case 200:
                        options.progress(100);
                        options.success();
                        break;
                    default:
                        options.error(Constants.ERROR_WEB_REQUEST);
                    }
                }, function (xhr, statusCode, timeout) {
                    options.error(Constants.ERROR_WEB_REQUEST);
                });
            };
            CallbackInvoker.invokeCallbackAsynchronous(pollForProgress, pollingInterval);
        };
        return Saver;
    }();
module.exports = Saver;
},{"../Constants":1,"../OneDriveState":4,"../models/ErrorType":6,"../models/SaverOptions":10,"../models/UploadType":11,"./AccountChooser":12,"./CallbackInvoker":13,"./ErrorHandler":15,"./Logging":17,"./ObjectUtilities":18,"./Popup":20,"./RedirectUtilities":21,"./StringUtilities":24,"./UrlUtilities":26,"./VroomWrapper":27,"./XHR":29}],24:[function(_dereq_,module,exports){
var FORMAT_ARGS_REGEX = /[\{\}]/g;
var FORMAT_REGEX = /\{\d+\}/g;
var StringUtilities = function () {
        function StringUtilities() {
        }
        StringUtilities.format = function (str) {
            var values = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                values[_i - 1] = arguments[_i];
            }
            var replacer = function (match) {
                var replacement = values[match.replace(FORMAT_ARGS_REGEX, '')];
                if (replacement === null) {
                    replacement = '';
                }
                return replacement;
            };
            return str.replace(FORMAT_REGEX, replacer);
        };
        return StringUtilities;
    }();
module.exports = StringUtilities;
},{}],25:[function(_dereq_,module,exports){
var Constants = _dereq_('../Constants'), ErrorHandler = _dereq_('./ErrorHandler'), Logging = _dereq_('./Logging'), ObjectUtilities = _dereq_('./ObjectUtilities'), StringUtilities = _dereq_('./StringUtilities');
var TypeValidators = function () {
        function TypeValidators() {
        }
        TypeValidators.validateType = function (object, expectedType, optional, defaultValue, validValues) {
            if (optional === void 0) {
                optional = false;
            }
            if (object === undefined) {
                if (optional) {
                    if (defaultValue === undefined) {
                        ErrorHandler.throwError('default value missing');
                    }
                    Logging.logMessage('applying default value: ' + defaultValue);
                    return defaultValue;
                } else {
                    ErrorHandler.throwError('object was missing and not optional');
                }
            }
            var objectType = typeof object;
            if (objectType !== expectedType) {
                ErrorHandler.throwError(StringUtilities.format('expected object type: \'{0}\', actual object type: \'{1}\'', expectedType, objectType));
            }
            if (!TypeValidators._isValidValue(object, validValues)) {
                ErrorHandler.throwError(StringUtilities.format('object does not match any valid values: \'{0}\'', ObjectUtilities.serializeJSON(validValues)));
            }
            return object;
        };
        TypeValidators.validateCallback = function (functionOption, optional, expectGlobalFunction) {
            if (optional === void 0) {
                optional = false;
            }
            if (expectGlobalFunction === void 0) {
                expectGlobalFunction = false;
            }
            if (functionOption === undefined) {
                if (optional) {
                    return null;
                } else {
                    ErrorHandler.throwError('function was missing and not optional');
                }
            }
            var functionType = typeof functionOption;
            if (functionType !== Constants.TYPE_STRING && functionType !== Constants.TYPE_FUNCTION) {
                ErrorHandler.throwError(StringUtilities.format('expected function type: \'function | string\', actual type: \'{0}\'', functionType));
            }
            var returnFunction = null;
            if (functionType === Constants.TYPE_STRING) {
                var globalFunction = window[functionOption];
                if (typeof globalFunction === Constants.TYPE_FUNCTION) {
                    returnFunction = globalFunction;
                } else {
                    ErrorHandler.throwError(StringUtilities.format('could not find a function with name \'{0}\' on the window object', functionOption));
                }
            } else if (expectGlobalFunction) {
                ErrorHandler.throwError('expected a global function');
            } else {
                returnFunction = functionOption;
            }
            return returnFunction;
        };
        TypeValidators._isValidValue = function (object, validValues) {
            if (!Array.isArray(validValues)) {
                return true;
            }
            for (var index in validValues) {
                if (object === validValues[index]) {
                    return true;
                }
            }
            return false;
        };
        return TypeValidators;
    }();
module.exports = TypeValidators;
},{"../Constants":1,"./ErrorHandler":15,"./Logging":17,"./ObjectUtilities":18,"./StringUtilities":24}],26:[function(_dereq_,module,exports){
var ErrorHandler = _dereq_('./ErrorHandler'), StringUtilities = _dereq_('./StringUtilities');
var UrlUtilities = function () {
        function UrlUtilities() {
        }
        UrlUtilities.appendToPath = function (baseUrl, path) {
            return baseUrl + (baseUrl.charAt(baseUrl.length - 1) !== '/' ? '/' : '') + path;
        };
        UrlUtilities.appendQueryString = function (baseUrl, queryKey, queryValue) {
            return UrlUtilities.appendQueryStrings(baseUrl, (_a = {}, _a[queryKey] = queryValue, _a));
            var _a;
        };
        UrlUtilities.appendQueryStrings = function (baseUrl, queryParameters) {
            if (!queryParameters || Object.keys(queryParameters).length === 0) {
                return baseUrl;
            }
            if (baseUrl.indexOf('?') === -1) {
                baseUrl += '?';
            } else if (baseUrl.charAt(baseUrl.length - 1) !== '&') {
                baseUrl += '&';
            }
            var queryString = '';
            for (var key in queryParameters) {
                queryString += (queryString.length ? '&' : '') + StringUtilities.format('{0}={1}', encodeURIComponent(key), encodeURIComponent(queryParameters[key]));
            }
            return baseUrl + queryString;
        };
        UrlUtilities.readCurrentUrlParameters = function () {
            return UrlUtilities.readUrlParameters(window.location.href);
        };
        UrlUtilities.readUrlParameters = function (url) {
            var queryParamters = {};
            var queryStart = url.indexOf('?') + 1;
            var hashStart = url.indexOf('#') + 1;
            if (queryStart > 0) {
                var queryEnd = hashStart > queryStart ? hashStart - 1 : url.length;
                UrlUtilities._deserializeParameters(url.substring(queryStart, queryEnd), queryParamters);
            }
            if (hashStart > 0) {
                UrlUtilities._deserializeParameters(url.substring(hashStart), queryParamters);
            }
            return queryParamters;
        };
        UrlUtilities.trimUrlQuery = function (url) {
            var separators = [
                    '?',
                    '#'
                ];
            for (var index in separators) {
                var charIndex = url.indexOf(separators[index]);
                if (charIndex > 0) {
                    url = url.substring(0, charIndex);
                }
            }
            return url;
        };
        UrlUtilities.getFileNameFromUrl = function (url) {
            var trimmedUrl = UrlUtilities.trimUrlQuery(url);
            return trimmedUrl.substr(trimmedUrl.lastIndexOf('/') + 1);
        };
        UrlUtilities.isPathFullUrl = function (path) {
            return path.indexOf('https://') === 0 || path.indexOf('http://') === 0;
        };
        UrlUtilities.isPathDataUrl = function (path) {
            return path.indexOf('data:') === 0;
        };
        UrlUtilities.validateUrlProtocol = function (url) {
            var protocols = [
                    'HTTP',
                    'HTTPS'
                ];
            for (var _i = 0; _i < protocols.length; _i++) {
                var protocol = protocols[_i];
                if (url.toUpperCase().indexOf(protocol) === 0) {
                    return;
                }
            }
            ErrorHandler.throwError('redirect uri does not match any protocol in ' + protocols);
        };
        UrlUtilities._deserializeParameters = function (query, queryParameters) {
            var properties = query.split('&');
            for (var i = 0; i < properties.length; i++) {
                var property = properties[i].split('=');
                if (property.length === 2) {
                    queryParameters[decodeURIComponent(property[0])] = decodeURIComponent(property[1]);
                }
            }
        };
        return UrlUtilities;
    }();
module.exports = UrlUtilities;
},{"./ErrorHandler":15,"./StringUtilities":24}],27:[function(_dereq_,module,exports){
var Constants = _dereq_('../Constants'), ErrorHandler = _dereq_('./ErrorHandler'), Logging = _dereq_('./Logging'), ObjectUtilities = _dereq_('./ObjectUtilities'), OneDriveState = _dereq_('../OneDriveState'), UrlUtilities = _dereq_('./UrlUtilities'), XHR = _dereq_('./XHR');
var VroomWrapper = function () {
        function VroomWrapper() {
        }
        VroomWrapper.callVroomOpen = function (response, getWebLinks, success, error) {
            VroomWrapper._callVroom(response, getWebLinks, false, success, error);
        };
        VroomWrapper.callVroomSave = function (response, success, error) {
            VroomWrapper._callVroom(response, false, true, success, error);
        };
        VroomWrapper._callVroom = function (response, getWebLinks, isSaver, success, error) {
            var apiEndpointUrl = response.apiEndpointUrl;
            var apiEndpoint = response.apiEndpoint;
            var accessToken = response.accessToken;
            var ownerCid = response.ownerCid;
            var itemId = response.itemId;
            var authKey = response.authKey;
            if (!authKey && getWebLinks) {
                ErrorHandler.throwError('missing auth key');
            }
            var queryParameters;
            if (getWebLinks) {
                apiEndpointUrl = UrlUtilities.appendToPath(apiEndpointUrl, 'drives/' + ownerCid + '/items/' + itemId);
                queryParameters = {
                    authKey: authKey,
                    expand: 'thumbnails,children(select=id,webUrl,name,size;expand=thumbnails)',
                    select: 'id,webUrl,name,size'
                };
            } else {
                apiEndpointUrl = UrlUtilities.appendToPath(apiEndpointUrl, 'drive/items/' + itemId + '/children');
                queryParameters = {};
                queryParameters[Constants.PARAM_ACCESS_TOKEN] = accessToken;
                if (isSaver) {
                    queryParameters['select'] = 'id';
                } else {
                    queryParameters['expand'] = 'thumbnails';
                    queryParameters['select'] = 'id,@content.downloadUrl,name,size';
                }
            }
            var xhr = new XHR({
                    url: UrlUtilities.appendQueryStrings(apiEndpointUrl, queryParameters),
                    clientId: OneDriveState.clientIds.msaClientId,
                    method: Constants.HTTP_GET,
                    apiEndpoint: apiEndpoint
                });
            Logging.logMessage('performing GET on sharing bundle with id: ' + itemId);
            xhr.start(function (xhr, statusCode) {
                success(ObjectUtilities.deserializeJSON(xhr.responseText));
            }, function (xhr, statusCode, timeout) {
                error();
            });
        };
        return VroomWrapper;
    }();
module.exports = VroomWrapper;
},{"../Constants":1,"../OneDriveState":4,"./ErrorHandler":15,"./Logging":17,"./ObjectUtilities":18,"./UrlUtilities":26,"./XHR":29}],28:[function(_dereq_,module,exports){
var Logging = _dereq_('./Logging'), ObjectUtilities = _dereq_('./ObjectUtilities');
var WindowState = function () {
        function WindowState() {
        }
        WindowState.getWindowState = function () {
            return ObjectUtilities.deserializeJSON(window.name || '{}');
        };
        WindowState.setWindowState = function (values, windowState) {
            if (windowState === void 0) {
                windowState = null;
            }
            if (windowState === null) {
                windowState = WindowState.getWindowState();
            }
            for (var property in values) {
                windowState[property] = values[property];
            }
            var serializedWindowState = ObjectUtilities.serializeJSON(windowState);
            Logging.logMessage('window.name = ' + serializedWindowState);
            window.name = serializedWindowState;
        };
        return WindowState;
    }();
module.exports = WindowState;
},{"./Logging":17,"./ObjectUtilities":18}],29:[function(_dereq_,module,exports){
var ApiEndpoint = _dereq_('../models/ApiEndpoint'), Constants = _dereq_('../Constants'), ErrorHandler = _dereq_('./ErrorHandler'), Logging = _dereq_('./Logging'), StringUtilities = _dereq_('./StringUtilities');
var REQUEST_TIMEOUT = 30000;
var EXCEPTION_STATUS = -1;
var TIMEOUT_STATUS = -2;
var ABORT_STATUS = -3;
var XHR = function () {
        function XHR(options) {
            this._url = options.url;
            this._json = options.json;
            this._headers = options.headers || {};
            this._method = options.method;
            this._clientId = options.clientId;
            this._apiEndpoint = options.apiEndpoint || ApiEndpoint.other;
            ErrorHandler.registerErrorObserver(this._abortRequest);
        }
        XHR.statusCodeToString = function (statusCode) {
            switch (statusCode) {
            case -1:
                return 'EXCEPTION';
            case -2:
                return 'TIMEOUT';
            case -3:
                return 'REQUEST ABORTED';
            default:
                return statusCode.toString();
            }
        };
        XHR.prototype.start = function (successCallback, failureCallback) {
            var _this = this;
            try {
                this._successCallback = successCallback;
                this._failureCallback = failureCallback;
                this._request = new XMLHttpRequest();
                this._request.ontimeout = this._onTimeout;
                this._request.onreadystatechange = function () {
                    if (!_this._completed && _this._request.readyState === 4) {
                        _this._completed = true;
                        var status_1 = _this._request.status;
                        if (status_1 < 400 && status_1 > 0) {
                            _this._callSuccessCallback(status_1);
                        } else {
                            _this._callFailureCallback(status_1);
                        }
                    }
                };
                if (!this._method) {
                    this._method = this._json ? Constants.HTTP_POST : Constants.HTTP_GET;
                }
                this._request.open(this._method, this._url, true);
                this._request.timeout = REQUEST_TIMEOUT;
                this._setHeaders();
                Logging.logMessage('starting request to: ' + this._url);
                this._request.send(this._json);
            } catch (error) {
                this._callFailureCallback(EXCEPTION_STATUS, error);
            }
        };
        XHR.prototype.upload = function (data, successCallback, failureCallback, progressCallback) {
            var _this = this;
            try {
                this._successCallback = successCallback;
                this._progressCallback = progressCallback;
                this._failureCallback = failureCallback;
                this._request = new XMLHttpRequest();
                this._request.ontimeout = this._onTimeout;
                this._method = Constants.HTTP_PUT;
                this._request.open(this._method, this._url, true);
                this._setHeaders();
                this._request.onload = function (event) {
                    _this._completed = true;
                    var status = _this._request.status;
                    if (status === 200 || status === 201) {
                        _this._callSuccessCallback(status);
                    } else {
                        _this._callFailureCallback(status, event);
                    }
                };
                this._request.onerror = function (event) {
                    _this._completed = true;
                    _this._callFailureCallback(_this._request.status, event);
                };
                this._request.upload.onprogress = function (event) {
                    if (event.lengthComputable) {
                        var uploadProgress = {
                                bytesTransferred: event.loaded,
                                totalBytes: event.total,
                                progressPercentage: event.total === 0 ? 0 : event.loaded / event.total * 100
                            };
                        _this._callProgressCallback(uploadProgress);
                    }
                };
                Logging.logMessage('starting upload to: ' + this._url);
                this._request.send(data);
            } catch (error) {
                this._callFailureCallback(EXCEPTION_STATUS, error);
            }
        };
        XHR.prototype._callSuccessCallback = function (status) {
            Logging.logMessage('calling xhr success callback, status: ' + XHR.statusCodeToString(status));
            this._successCallback(this._request, status, this._url);
        };
        XHR.prototype._callFailureCallback = function (status, error) {
            Logging.logError('calling xhr failure callback, status: ' + XHR.statusCodeToString(status), this._request, error);
            this._failureCallback(this._request, status, status === TIMEOUT_STATUS);
        };
        XHR.prototype._callProgressCallback = function (uploadProgress) {
            Logging.logMessage('calling xhr upload progress callback');
            this._progressCallback(this._request, uploadProgress);
        };
        XHR.prototype._abortRequest = function () {
            if (!this._completed) {
                this._completed = true;
                if (this._request) {
                    try {
                        this._request.abort();
                    } catch (error) {
                    }
                }
                this._callFailureCallback(ABORT_STATUS);
            }
        };
        XHR.prototype._onTimeout = function () {
            if (!this._completed) {
                this._completed = true;
                this._callFailureCallback(TIMEOUT_STATUS);
            }
        };
        XHR.prototype._setHeaders = function () {
            for (var x in this._headers) {
                this._request.setRequestHeader(x, this._headers[x]);
            }
            if (this._clientId && this._apiEndpoint !== ApiEndpoint.other) {
                this._request.setRequestHeader('Application', '0x' + this._clientId);
            }
            var sdkVersion = StringUtilities.format('{0}={1}', 'SDK-Version', Constants.SDK_VERSION);
            switch (this._apiEndpoint) {
            case ApiEndpoint.filesV2:
                this._request.setRequestHeader('X-ClientService-ClientTag', sdkVersion);
                break;
            case ApiEndpoint.vroom:
                this._request.setRequestHeader('X-RequestStats', sdkVersion);
                break;
            case ApiEndpoint.other:
                break;
            default:
                ErrorHandler.throwError('invalid API endpoint: ' + this._apiEndpoint);
            }
            if (this._method === Constants.HTTP_POST) {
                this._request.setRequestHeader('Content-Type', this._json ? 'application/json' : 'text/plain');
            }
        };
        return XHR;
    }();
module.exports = XHR;
},{"../Constants":1,"../models/ApiEndpoint":5,"./ErrorHandler":15,"./Logging":17,"./StringUtilities":24}]},{},[2])
(2)
});