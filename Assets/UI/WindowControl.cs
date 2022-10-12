using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace YX
{
    public class WindowControl
    {
        private static Win32Export.WINDOWPLACEMENT _placement;
        public static Win32Export.WStyle CurrentStyle
        {
            get
            {
                _placement.length = (uint)Marshal.SizeOf<Win32Export.WINDOWPLACEMENT>();
                var s = Win32Export.GetWindowPlacement(_hWnd, ref _placement);
                UnityEngine.Debug.Assert(s, "GetWindowPlacement失败");
                UnityEngine.Debug.LogWarning("CurrentStyle:" + _placement.showCmd);
                switch (_placement.showCmd)
                {
                    case 0:
                        return Win32Export.WStyle.HIDE;
                    case 1:
                        return Win32Export.WStyle.NORMAL;
                    case 6:
                        return Win32Export.WStyle.MINISIZE;
                    case 3:
                        return Win32Export.WStyle.MAXSIZE;
                    default:
                        return Win32Export.WStyle.NORMAL;
                }
            }
        }
        public static UnityAction<Win32Export.WStyle> OnWindowModeChangeCallback;

        private static IntPtr _hWnd;
        private static List<GameObject> _hitObjects = new List<GameObject>(5);

        public static void Init()
        {
            _hWnd = Win32Export.GetCurrentWindowHandle();
            if (Application.isEditor)
                return;
        }

        public static void InitFullNormal()
        {
            if (Application.isEditor)
                return;

            OnlyFrame();
            //Win32Export.ShowWindow(_hWnd, (int)Win32Export.WStyle.NORMAL);
            int x, y, w, h;
            x = y = w = h = 0;
            w = Win32Export.GetSystemMetrics(Win32Export.SM_CXSCREEN);
            h = Win32Export.GetSystemMetrics(Win32Export.SM_CYSCREEN);
            Win32Export.SetWindowPos(_hWnd, Win32Export.HWND_NORMAL, x, y, w, h, Win32Export.SWP_NOMOVE);
            OnWindowModeChangeCallback?.Invoke(CurrentStyle);
        }

        public static void Normal()
        {
            _drag = false;
            if (Application.isEditor)
                return;

            OnlyFrame();
            Win32Export.ShowWindow(_hWnd, (int)Win32Export.WStyle.NORMAL);
            /*int x, y, w, h;
            x = y = w = h = 0;
            GetNormalSize(ref x, ref y, ref w, ref h);
            Win32Export.SetWindowPos(_hWnd, Win32Export.HWND_NORMAL, x, y, w, h, Win32Export.SWP_SHOWWINDOW | Win32Export.SWP_NOMOVE);*/
            OnWindowModeChangeCallback?.Invoke(CurrentStyle);
        }

        public static void Min()
        {
            _drag = false;
            if (Application.isEditor)
                return;

            SetNormalSize();
            Win32Export.ShowWindow(_hWnd, (int)Win32Export.WStyle.MINISIZE);
            //Win32.SetWindowPos(_hWnd, -2, x, y, cx, cy, nflags);
            OnWindowModeChangeCallback?.Invoke(CurrentStyle);
        }

        public static void Max()
        {
            _drag = false;
            if (Application.isEditor)
                return;

            SetNormalSize();
            NoFrame();
            Win32Export.ShowWindow(_hWnd, (int)Win32Export.WStyle.MAXSIZE);
            //Win32.SetWindowPos(_hWnd, -2, x, y, cx, cy, nflags);
            OnWindowModeChangeCallback?.Invoke(CurrentStyle);
        }

        public static void AddMouseTrigger(GameObject obj)
        {
            if (!_hitObjects.Contains(obj))
            {
                _hitObjects.Add(obj);
            }
        }

        public static void RemoveMouseTrigger(GameObject obj)
        {
            if (_hitObjects.Contains(obj))
            {
                _hitObjects.Remove(obj);
            }
        }

        private static bool IsHoverTrigger()
        {
            for (int i = 0; i < _hitObjects.Count; i++)
            {
                var obj = _hitObjects[i];
                if (obj == null)
                    continue;

                var rectTran = obj.GetComponent<RectTransform>();
                if (rectTran != null)
                {
                    var canvas = rectTran.GetComponentInParent<Canvas>();
                    Camera cam = null;
                    if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
                        cam = canvas.worldCamera;

                    if (RectTransformUtility.RectangleContainsScreenPoint(rectTran, Input.mousePosition, cam))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static float _mouseX = 0;
        static float _mouseY = 0;
        static bool _drag = false;
        public static void Update()
        {
            if (Input.GetMouseButtonDown(0) && IsHoverTrigger() && CurrentStyle == Win32Export.WStyle.NORMAL)
                _drag = true;
            else if (Input.GetMouseButtonUp(0))
                _drag = false;

            if (_drag)
            {
                var dx = Input.mousePosition.x - _mouseX;
                var dy = Input.mousePosition.y - _mouseY;
                Win32Export.RECT wndRect = Win32Export.GetWndRect(_hWnd);
                float x = wndRect.left;
                x += dx;
                float y = wndRect.top;
                y -= dy;
                var width = wndRect.right - wndRect.left;
                var height = wndRect.bottom - wndRect.top;
                if (!Application.isEditor)
                {
                    Win32Export.MoveWindow(_hWnd, (int)x, (int)y, width, height, true);
                }
                _mouseX = Input.mousePosition.x - dx;
                _mouseY = Input.mousePosition.y - dy;
                return;
            }
            _mouseX = Input.mousePosition.x;
            _mouseY = Input.mousePosition.y;
        }

        private static void OnlyFrame()
        {
            if (Application.isEditor)
                return;

            long style = Win32Export.GetWindowLong(_hWnd, Win32Export.GWL_STYLE);
            style = style & ~Win32Export.WS_CAPTION & ~Win32Export.WS_SYSMENU | Win32Export.WS_SIZEBOX;
            Win32Export.SetWindowLong(_hWnd, Win32Export.GWL_STYLE, style);
        }

        private static void NoFrame()
        {
            if (Application.isEditor)
                return;

            long style = Win32Export.GetWindowLong(_hWnd, Win32Export.GWL_STYLE);
            style = style & ~Win32Export.WS_CAPTION & ~Win32Export.WS_SYSMENU & ~Win32Export.WS_SIZEBOX;
            Win32Export.SetWindowLong(_hWnd, Win32Export.GWL_STYLE, style);
        }

        private static void SetNormalSize()
        {
            Win32Export.RECT wndRect = Win32Export.GetWndRect(_hWnd);
            //Win32Export.RECT workArea = Win32Export.GetWorkArea(Win32Export.NewPoint(wndRect.left, wndRect.top));
            var x = wndRect.left;
            var y = wndRect.top;
            var w = wndRect.right - wndRect.left;
            var h = wndRect.bottom - wndRect.top;

            PlayerPrefs.SetInt("Screenmanager Resolution Width", w);
            PlayerPrefs.SetInt("Screenmanager Resolution Height", h);
        }

        private static void GetNormalSize(ref int x, ref int y, ref int w, ref int h)
        {
            Win32Export.RECT wndRect = Win32Export.GetWndRect(_hWnd);
            //Win32Export.RECT workArea = Win32Export.GetWorkArea(Win32Export.NewPoint(wndRect.left, wndRect.top));
            var dx = wndRect.left;
            var dy = wndRect.top;
            var dw = wndRect.right - wndRect.left;
            var dh = wndRect.bottom - wndRect.top;

            w = PlayerPrefs.GetInt("Screenmanager Resolution Width", dw);
            h = PlayerPrefs.GetInt("Screenmanager Resolution Height", dh);
        }


    }

    public static class Win32Export
    {
        public const int HWND_NORMAL = -2;

        public const int SWP_SHOWWINDOW = 64;
        public const int SWP_NOMOVE = 2;
        public const int SWP_NOSIZE = 1;

        public const uint WS_CAPTION = 12582912u;
        public const uint WS_SYSMENU = 524288u;
        public const uint WS_SIZEBOX = 262144u;

        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        public const int GWL_STYLE = -16;


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;

            public int y;
        }

        public struct RECT
        {
            public int left;

            public int top;

            public int right;

            public int bottom;
        }

        public enum WStyle
        {
            HIDE,
            NORMAL,
            MAXSIZE = 3,
            MINISIZE = 6,
            FULLSCREEN = 10001
        }

        public struct WINDOWPLACEMENT
        {
            public uint length;
            public uint flags;
            public uint showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
            public RECT rcDevice;
        }

        [DllImport("user32.dll")]
        public extern static bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool bRepaint);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong)
        {
            IntPtr zero = IntPtr.Zero;
            return (IntPtr.Size != 8) ? new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong)) : SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
        public extern static int SetWindowLong32(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
        public extern static IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll")]
        public extern static uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint nflags);

        public static RECT GetWndRect(IntPtr hWnd)
        {
            RECT result = default(RECT);
            GetWindowRect(hWnd, ref result);
            return result;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static bool GetWindowRect(IntPtr hWnd, ref RECT pos);

        public static IntPtr GetCurrentWindowHandle()
        {
            IntPtr result = IntPtr.Zero;
            uint id = (uint)Process.GetCurrentProcess().Id;
            if (!EnumWindows(EnumWindowsProc, id) && Marshal.GetLastWin32Error() == 0)
            {
                if (_enumReturn != null)
                {
                    result = _enumReturn;
                }
            }
            return result;
        }

        private static IntPtr _enumReturn;

        private static bool EnumWindowsProc(IntPtr hwnd, uint lParam)
        {
            uint num = 0u;
            if (GetParent(hwnd) == IntPtr.Zero)
            {
                GetWindowThreadProcessId(hwnd, ref num);
                if (num == lParam)
                {
                    _enumReturn = hwnd;
                    SetLastError(0u);
                    return false;
                }
            }
            return true;
        }

        public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public extern static bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public extern static IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public extern static uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        public extern static void SetLastError(uint errorCode);

        [DllImport("user32.dll")]
        public extern static int GetSystemMetrics(int index);
    }
}
