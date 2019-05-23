function OpenTab(title) {
        var ret = {
            title: title,
            iniframe: true,
            closable: true,
            refreshable: false,
            iconCls: 'icon-standard-application-side-list',
            repeatable: false,
            selected: true
        };

        ref = parent.window.mainpage.mainTabs.isExists(title);
        parent.window.mainpage.mainTabs.addModuleTab($.extend(parent.window.mainpage.tabDefaultOption, ret));
        if (ref == 2) {
            parent.window.$("#mainTabs").tabs("getSelected").find("iframe")[0].contentWindow
                .ReloadDataGridByName(name, pid, tenantName);
        }
    
}


function CloseTab() {
    if (parent.window.mainpage) {
        parent.window.mainpage.mainTabs.closeCurrentTab();
    } else if (parent) {
        location.href = "/";
    }
    
}
