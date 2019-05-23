var ajaxDefaultSetting = {
    cache: false,
    error: function(jqXHR, textStatus, errorThrown) {
        var status = 0;
        var message = '';
        switch (jqXHR.status) {
        case (500):
            //TODO 服务器系统内部错误
            status = 500;
            break;
        case (401):
            //TODO 未登录
            status = 401;
            message = '登录超时，请重新登录！';
            break;
        case (403):
            //TODO 无权限执行此操作
            status = 403;
            message = '抱歉,你不具有当前操作的权限！';
            break;
        case (408):
            //TODO 请求超时
            status = 408;
            break;
        case (0):
            //TODO cancel
            break;
        default:
            status = 1;
        //TODO 未知错误
        }
        if (status > 0) {
            $.easyui.loaded();
            if (status === 403) {
                $.easyui.messager.alert('系统提示', message.length ? message : errorThrown + '请联系网站管理员，错误代码：' + jqXHR.status, 'error', function () {
                });
            } else {
                $.messager.alert('系统提示', message.length ? message : errorThrown + '请联系网站管理员，错误代码：' + jqXHR.status, 'error', function () {
                    //window.location.href = window.location.href;
                });
            }
        }
    }
};

$.ajaxSetup(ajaxDefaultSetting);

$(document).ajaxError(function (event, jqXhr) {
    if (typeof (jqXhr) != 'undefined') {
        var resText = jqXhr.responseText;
        if (resText.indexOf('wa=wsignin1.0&wtrealm=') > 0) {
            top.location.reload(true);
        }
    }

});

function setHeight() {
    var c = $('.child-layout');
    if (c.length) {
        var p = c.layout('panel', 'center');
        if (p.length) {
            var oldHeight = p.panel('panel').outerHeight();
            p.panel('resize', { height: 'auto' });
            var newHeight = p.panel('panel').outerHeight();
            c.layout('resize', {
                height: (c.height() + newHeight - oldHeight)
            });
        }
    }
}

function setWidth() {
    var c = $('.child-layout');
    if (c.length) {
        c.layout('resize', {
            width: $('.child-body').width()
        });
    }
}

$(function () {
    $(window).resize(function () {
        setWidth();
        setHeight();
    });
})