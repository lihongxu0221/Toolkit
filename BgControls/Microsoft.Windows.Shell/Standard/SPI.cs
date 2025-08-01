using System;

namespace Standard;

internal enum SPI
{
    GETBEEP = 1,
    SETBEEP,
    GETMOUSE,
    SETMOUSE,
    GETBORDER,
    SETBORDER,
    GETKEYBOARDSPEED = 10,
    SETKEYBOARDSPEED,
    LANGDRIVER,
    ICONHORIZONTALSPACING,
    GETSCREENSAVETIMEOUT,
    SETSCREENSAVETIMEOUT,
    GETSCREENSAVEACTIVE,
    SETSCREENSAVEACTIVE,
    GETGRIDGRANULARITY,
    SETGRIDGRANULARITY,
    SETDESKWALLPAPER,
    SETDESKPATTERN,
    GETKEYBOARDDELAY,
    SETKEYBOARDDELAY,
    ICONVERTICALSPACING,
    GETICONTITLEWRAP,
    SETICONTITLEWRAP,
    GETMENUDROPALIGNMENT,
    SETMENUDROPALIGNMENT,
    SETDOUBLECLKWIDTH,
    SETDOUBLECLKHEIGHT,
    GETICONTITLELOGFONT,
    SETDOUBLECLICKTIME,
    SETMOUSEBUTTONSWAP,
    SETICONTITLELOGFONT,
    GETFASTTASKSWITCH,
    SETFASTTASKSWITCH,
    SETDRAGFULLWINDOWS,
    GETDRAGFULLWINDOWS,
    GETNONCLIENTMETRICS = 41,
    SETNONCLIENTMETRICS,
    GETMINIMIZEDMETRICS,
    SETMINIMIZEDMETRICS,
    GETICONMETRICS,
    SETICONMETRICS,
    SETWORKAREA,
    GETWORKAREA,
    SETPENWINDOWS,
    GETHIGHCONTRAST = 66,
    SETHIGHCONTRAST,
    GETKEYBOARDPREF,
    SETKEYBOARDPREF,
    GETSCREENREADER,
    SETSCREENREADER,
    GETANIMATION,
    SETANIMATION,
    GETFONTSMOOTHING,
    SETFONTSMOOTHING,
    SETDRAGWIDTH,
    SETDRAGHEIGHT,
    SETHANDHELD,
    GETLOWPOWERTIMEOUT,
    GETPOWEROFFTIMEOUT,
    SETLOWPOWERTIMEOUT,
    SETPOWEROFFTIMEOUT,
    GETLOWPOWERACTIVE,
    GETPOWEROFFACTIVE,
    SETLOWPOWERACTIVE,
    SETPOWEROFFACTIVE,
    SETCURSORS,
    SETICONS,
    GETDEFAULTINPUTLANG,
    SETDEFAULTINPUTLANG,
    SETLANGTOGGLE,
    GETWINDOWSEXTENSION,
    SETMOUSETRAILS,
    GETMOUSETRAILS,
    SETSCREENSAVERRUNNING = 97,
    SCREENSAVERRUNNING = 97,
    GETFILTERKEYS = 50,
    SETFILTERKEYS,
    GETTOGGLEKEYS,
    SETTOGGLEKEYS,
    GETMOUSEKEYS,
    SETMOUSEKEYS,
    GETSHOWSOUNDS,
    SETSHOWSOUNDS,
    GETSTICKYKEYS,
    SETSTICKYKEYS,
    GETACCESSTIMEOUT,
    SETACCESSTIMEOUT,
    GETSERIALKEYS,
    SETSERIALKEYS,
    GETSOUNDSENTRY,
    SETSOUNDSENTRY,
    GETSNAPTODEFBUTTON = 95,
    SETSNAPTODEFBUTTON,
    GETMOUSEHOVERWIDTH = 98,
    SETMOUSEHOVERWIDTH,
    GETMOUSEHOVERHEIGHT,
    SETMOUSEHOVERHEIGHT,
    GETMOUSEHOVERTIME,
    SETMOUSEHOVERTIME,
    GETWHEELSCROLLLINES,
    SETWHEELSCROLLLINES,
    GETMENUSHOWDELAY,
    SETMENUSHOWDELAY,
    GETWHEELSCROLLCHARS,
    SETWHEELSCROLLCHARS,
    GETSHOWIMEUI,
    SETSHOWIMEUI,
    GETMOUSESPEED,
    SETMOUSESPEED,
    GETSCREENSAVERRUNNING,
    GETDESKWALLPAPER,
    GETAUDIODESCRIPTION,
    SETAUDIODESCRIPTION,
    GETSCREENSAVESECURE,
    SETSCREENSAVESECURE,
    GETHUNGAPPTIMEOUT,
    SETHUNGAPPTIMEOUT,
    GETWAITTOKILLTIMEOUT,
    SETWAITTOKILLTIMEOUT,
    GETWAITTOKILLSERVICETIMEOUT,
    SETWAITTOKILLSERVICETIMEOUT,
    GETMOUSEDOCKTHRESHOLD,
    SETMOUSEDOCKTHRESHOLD,
    GETPENDOCKTHRESHOLD,
    SETPENDOCKTHRESHOLD,
    GETWINARRANGING,
    SETWINARRANGING,
    GETMOUSEDRAGOUTTHRESHOLD,
    SETMOUSEDRAGOUTTHRESHOLD,
    GETPENDRAGOUTTHRESHOLD,
    SETPENDRAGOUTTHRESHOLD,
    GETMOUSESIDEMOVETHRESHOLD,
    SETMOUSESIDEMOVETHRESHOLD,
    GETPENSIDEMOVETHRESHOLD,
    SETPENSIDEMOVETHRESHOLD,
    GETDRAGFROMMAXIMIZE,
    SETDRAGFROMMAXIMIZE,
    GETSNAPSIZING,
    SETSNAPSIZING,
    GETDOCKMOVING,
    SETDOCKMOVING,
    GETACTIVEWINDOWTRACKING = 4096,
    SETACTIVEWINDOWTRACKING,
    GETMENUANIMATION,
    SETMENUANIMATION,
    GETCOMBOBOXANIMATION,
    SETCOMBOBOXANIMATION,
    GETLISTBOXSMOOTHSCROLLING,
    SETLISTBOXSMOOTHSCROLLING,
    GETGRADIENTCAPTIONS,
    SETGRADIENTCAPTIONS,
    GETKEYBOARDCUES,
    SETKEYBOARDCUES,
    GETMENUUNDERLINES = 4106,
    SETMENUUNDERLINES,
    GETACTIVEWNDTRKZORDER,
    SETACTIVEWNDTRKZORDER,
    GETHOTTRACKING,
    SETHOTTRACKING,
    GETMENUFADE = 4114,
    SETMENUFADE,
    GETSELECTIONFADE,
    SETSELECTIONFADE,
    GETTOOLTIPANIMATION,
    SETTOOLTIPANIMATION,
    GETTOOLTIPFADE,
    SETTOOLTIPFADE,
    GETCURSORSHADOW,
    SETCURSORSHADOW,
    GETMOUSESONAR,
    SETMOUSESONAR,
    GETMOUSECLICKLOCK,
    SETMOUSECLICKLOCK,
    GETMOUSEVANISH,
    SETMOUSEVANISH,
    GETFLATMENU,
    SETFLATMENU,
    GETDROPSHADOW,
    SETDROPSHADOW,
    GETBLOCKSENDINPUTRESETS,
    SETBLOCKSENDINPUTRESETS,
    GETUIEFFECTS = 4158,
    SETUIEFFECTS,
    GETDISABLEOVERLAPPEDCONTENT,
    SETDISABLEOVERLAPPEDCONTENT,
    GETCLIENTAREAANIMATION,
    SETCLIENTAREAANIMATION,
    GETCLEARTYPE = 4168,
    SETCLEARTYPE,
    GETSPEECHRECOGNITION,
    SETSPEECHRECOGNITION,
    GETFOREGROUNDLOCKTIMEOUT = 8192,
    SETFOREGROUNDLOCKTIMEOUT,
    GETACTIVEWNDTRKTIMEOUT,
    SETACTIVEWNDTRKTIMEOUT,
    GETFOREGROUNDFLASHCOUNT,
    SETFOREGROUNDFLASHCOUNT,
    GETCARETWIDTH,
    SETCARETWIDTH,
    GETMOUSECLICKLOCKTIME,
    SETMOUSECLICKLOCKTIME,
    GETFONTSMOOTHINGTYPE,
    SETFONTSMOOTHINGTYPE,
    GETFONTSMOOTHINGCONTRAST,
    SETFONTSMOOTHINGCONTRAST,
    GETFOCUSBORDERWIDTH,
    SETFOCUSBORDERWIDTH,
    GETFOCUSBORDERHEIGHT,
    SETFOCUSBORDERHEIGHT,
    GETFONTSMOOTHINGORIENTATION,
    SETFONTSMOOTHINGORIENTATION,
    GETMINIMUMHITRADIUS,
    SETMINIMUMHITRADIUS,
    GETMESSAGEDURATION,
    SETMESSAGEDURATION
}
