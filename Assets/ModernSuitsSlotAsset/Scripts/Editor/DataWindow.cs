using UnityEngine;
using UnityEditor;

namespace Mkey
{
    public class DataWindow : EditorWindow
    {
        private GameObject currentSelection;
        private static string[,] table;
        private static string[,] table_1;
        private static float sum, sumFreeSpins;
        private static DataWindow dataWindow;
        public static void Init()
        {
            
            var window = (DataWindow)GetWindow(typeof(DataWindow));
            dataWindow = window;
            var content = new GUIContent();
            content.text = "Probabilities";

            var icon = new Texture2D(16, 16);
            content.image = icon;

            window.titleContent = content;
        }

        private void OnFocus()
        {
            currentSelection = Selection.activeGameObject;
        }

        private void OnLostFocus()
        {
            currentSelection = null;
        }

        public static void SetData(string [,] t, string[,] t_1, float s, float sFS)
        {
            Debug.Log("set");
            table = t;
            table_1 = t_1;
            sum = s;
            sumFreeSpins = sFS;
            if (dataWindow) dataWindow.Repaint();
        }

        Vector2 scrollPos;
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));
            #region test
            // DrawSelectionGridColumn();
            // DrawSelectionGridTable(2, 2);
            // DrawArea();
            // DrawPassField();
            // DrawRepeatButton();
            // DrawTextArea();
            // DrawTextField();
            // DrawTools();
            // DrawToogle();
            // DrawAreaScope();
            // DrawEGLabel("label_1", "label_2", new Rect(100,400, 100, EditorGUIUtility.singleLineHeight));
            // CreateRow(new string[] { "1", "2", "3", "4", "5" }, 100, 20, 50, 0);
            // CreateRow(new string[] { "1", "2", "3", "4", "5" }, 100, 20, 50, 0);
            // CreateRow(new string[] { "1", "2", "3", "4", "5" }, 100, 20, 50, 0);
            // if (GUILayout.Button("Calculate"))
                //  myScript.SeconButton_Click();
            #endregion test

            #region table
            if (table != null)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Probabilities: ", new GUIStyle(EditorStyles.boldLabel));
                CreateTable(table, 80, 0,0);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            #endregion table

            #region table_1
            if (table_1 != null)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Paylines: ", new GUIStyle(EditorStyles.boldLabel));
                CreateTable(table_1, 120, 0, 0);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Return, %: " + sum.ToString("F4"), new GUIStyle(EditorStyles.boldLabel));
                EditorGUILayout.LabelField("Return (with free spins effect), %: " + sumFreeSpins.ToString("F4"), new GUIStyle(EditorStyles.boldLabel));
            }
            #endregion table_1

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Press calculate ");


            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        #region selection grid
        //https://docs.unity3d.com/ScriptReference/GUILayout.SelectionGrid.html
        int selGridInt = 0;
        string[] selStrings = new string[] { "radio1", "radio2", "radio3", "radio4" };
        void DrawSelectionGridColumn()
        {
            GUILayout.BeginVertical("Box");
            selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 1);
            if (GUILayout.Button("Start"))
                Debug.Log("You chose " + selStrings[selGridInt]);

            GUILayout.EndVertical();
        }
        void DrawSelectionGridTable(int r, int c)
        {
            GUILayout.Space(10);
            GUILayout.BeginVertical("Box");
            selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, c);
            if (GUILayout.Button("Start"))
                Debug.Log("You chose " + selStrings[selGridInt]);

            GUILayout.EndVertical();
        }

        #endregion selection grid

        #region area
        void DrawArea() // fixed area (положение фиксировано в координатах окна)
        {
            //https://docs.unity3d.com/ScriptReference/GUILayout.BeginArea.html
            /*
             Overloads:
                public static void BeginArea(Rect screenRect, string text);
                public static void BeginArea(Rect screenRect, Texture image);
                public static void BeginArea(Rect screenRect, GUIContent content);
                public static void BeginArea(Rect screenRect, GUIStyle style);
                public static void BeginArea(Rect screenRect, string text, GUIStyle style);
                public static void BeginArea(Rect screenRect, Texture image, GUIStyle style);
                public static void BeginArea(Rect screenRect, GUIContent content, GUIStyle style); 
             */
            GUILayout.BeginArea(new Rect(10, 10, 100, 100));
            GUILayout.Button("Click me");
            GUILayout.Button("Or me");
            GUILayout.EndArea();
        }
        #endregion area

        #region pass 
        public string passwordToEdit = "My Password";
        void DrawPassField()
        {
            //https://docs.unity3d.com/ScriptReference/GUILayout.PasswordField.html
            /*
             Overloads:
                public static string PasswordField(string password, char maskChar, int maxLength, params GUILayoutOption[] options);
                public static string PasswordField(string password, char maskChar, GUIStyle style, params GUILayoutOption[] options);
                public static string PasswordField(string password, char maskChar, int maxLength, GUIStyle style, params GUILayoutOption[] options); 
             */
            passwordToEdit = GUILayout.PasswordField(passwordToEdit, "*"[0], 25);
        }
        #endregion pass

        #region repeatbutton
        Texture tex;
        void DrawRepeatButton()
        {  //return true when the holds down the mouse https://docs.unity3d.com/ScriptReference/GUILayout.RepeatButton.html
            /*
             Overloads:
                public static bool RepeatButton(Texture image, params GUILayoutOption[] options);
                public static bool RepeatButton(string text, params GUILayoutOption[] options);
                public static bool RepeatButton(GUIContent content, params GUILayoutOption[] options);
                public static bool RepeatButton(Texture image, GUIStyle style, params GUILayoutOption[] options);
                public static bool RepeatButton(string text, GUIStyle style, params GUILayoutOption[] options);
                public static bool RepeatButton(GUIContent content, GUIStyle style, params GUILayoutOption[] options); 
             */
            if (GUILayout.RepeatButton("I am a regular Automatic Layout Button"))
            Debug.Log("Clicked Button");

            if (!tex)
            {
                tex = FindFirsTexture("editor_icon");
            }

            if (!tex)
            {
                Debug.LogError("No texture found");
            }
            else
            {
                if (GUILayout.RepeatButton(tex))
                    Debug.Log("Clicked the image");
            }
        }
        #endregion repeatbutton

        #region window
        void DrawWindow()// not work in editor window ???
        {
            //https://docs.unity3d.com/ScriptReference/GUILayout.Window.html
            /*
             Windows float above normal GUI controls, feature click-to-focus and can optionally be dragged around by the end user. 
             Unlike other controls, you need to pass them a separate function for the GUI controls to put inside the window.
             Overloads:
                public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, string text, params GUILayoutOption[] options);
                public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, Texture image, params GUILayoutOption[] options);
                public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, params GUILayoutOption[] options);
                public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, string text, GUIStyle style, params GUILayoutOption[] options);
                public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, Texture image, GUIStyle style, params GUILayoutOption[] options);
                public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, GUIStyle style, params GUILayoutOption[] options); 
             */
            Rect windowRect = new Rect(100, 22, 120, 120);
            windowRect = GUILayout.Window(0, windowRect, DoMyWindow, "My Window");
        } 

        void DoMyWindow(int windowID)
        {
            if (GUILayout.Button("Please click me a lot"))
               Debug.Log("Got a click");
        }
        #endregion window

        #region textarea
        string stringToEdit = "Hello World\nI've got 2 lines...";
        void DrawTextArea()
        {
            //https://docs.unity3d.com/ScriptReference/GUILayout.TextArea.html
            //Make a multi-line text field where the user can edit a string.
            /*
             public static string TextArea(string text, params GUILayoutOption[] options);
             public static string TextArea(string text, int maxLength, params GUILayoutOption[] options);
             public static string TextArea(string text, GUIStyle style, params GUILayoutOption[] options);
             public static string TextArea(string text, int maxLength, GUIStyle style, params GUILayoutOption[] options); 
             */

            stringToEdit = GUILayout.TextArea(stringToEdit, 200);
        }
        #endregion textarea

        #region textfield
        public string singleStringToEdit = "Hello World";
        void DrawTextField()
        {
            //https://docs.unity3d.com/ScriptReference/GUILayout.TextField.html
            //Make a single-line text field where the user can edit a string.
            /*
             Overloads:
                public static string TextField(string text, params GUILayoutOption[] options);
                public static string TextField(string text, int maxLength, params GUILayoutOption[] options);
                public static string TextField(string text, GUIStyle style, params GUILayoutOption[] options);
                public static string TextField(string text, int maxLength, GUIStyle style, params GUILayoutOption[] options); 
             */
            singleStringToEdit = GUILayout.TextField(singleStringToEdit, 25);
        }
        #endregion textfield

        #region tools
        int toolbarInt = 0;
        string[] toolbarStrings = new string[] { "Toolbar1", "Toolbar2", "Toolbar3" };
        void DrawTools()
        {
            //https://docs.unity3d.com/ScriptReference/GUILayout.Toolbar.html
            //Make a toolbar.
            /*
             Overloads:
                public static int Toolbar(int selected, string[] texts, params GUILayoutOption[] options);
                public static int Toolbar(int selected, Texture[] images, params GUILayoutOption[] options);
                public static int Toolbar(int selected, GUIContent[] contents, params GUILayoutOption[] options);
                public static int Toolbar(int selected, string[] texts, GUIStyle style, params GUILayoutOption[] options);
                public static int Toolbar(int selected, Texture[] images, GUIStyle style, params GUILayoutOption[] options);
                public static int Toolbar(int selected, GUIContent[] contents, GUIStyle style, params GUILayoutOption[] options);
                public static int Toolbar(int selected, string[] texts, GUIStyle style, GUI.ToolbarButtonSize buttonSize, params GUILayoutOption[] options);
                public static int Toolbar(int selected, Texture[] images, GUIStyle style, GUI.ToolbarButtonSize buttonSize, params GUILayoutOption[] options);
                public static int Toolbar(int selected, GUIContent[] contents, GUIStyle style, GUI.ToolbarButtonSize buttonSize, params GUILayoutOption[] options); 
             */
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
        }
        #endregion tools

        #region toogle
        Texture aTexture;
        bool toggleTxt = false;
        bool toggleImg = false;
        void DrawToogle()
        {
            //https://docs.unity3d.com/ScriptReference/GUILayout.Toggle.html
            //Make an on/off toggle button.
            /*
             Overloads:
                public static bool Toggle(bool value, Texture image, params GUILayoutOption[] options);
                public static bool Toggle(bool value, string text, params GUILayoutOption[] options);
                public static bool Toggle(bool value, GUIContent content, params GUILayoutOption[] options);
                public static bool Toggle(bool value, Texture image, GUIStyle style, params GUILayoutOption[] options);
                public static bool Toggle(bool value, string text, GUIStyle style, params GUILayoutOption[] options);
                public static bool Toggle(bool value, GUIContent content, GUIStyle style, params GUILayoutOption[] options); 
             */
            if (!aTexture)
            {
                aTexture = FindFirsTexture("editor_icon");
            }
            if (!aTexture)
            {
                Debug.LogError("Please assign a texture in the inspector.");
                return;
            }
            else
            {
                toggleImg = GUILayout.Toggle(toggleImg, aTexture);
            }
            toggleTxt = GUILayout.Toggle(toggleTxt, "A Toggle text");
        }
        #endregion toogle

        #region areascope
        void DrawAreaScope() //work in editor window
        {
            //https://docs.unity3d.com/ScriptReference/GUILayout.AreaScope.html
            //https://docs.unity3d.com/ScriptReference/GUILayout.HorizontalScope.html
            //
            /*
             Overloads:

             */
            using (var areaScope = new GUILayout.AreaScope(new Rect(100, 100, 100, 100)))
            {
                GUILayout.Button("Click me area scope");
                GUILayout.Button("Or me Click me area scope");
            }

            using (var horizontalScope = new GUILayout.HorizontalScope("box"))
            {
                GUILayout.Button("I'm the first button");
                GUILayout.Button("I'm to the right");
            }

            using (var verticalScope = new GUILayout.VerticalScope("box"))
            {
                GUILayout.Button("I'm the top button");
                GUILayout.Button("I'm the bottom button");
            }
        }
        #endregion areascope

        #region editorguilo
        void DrawEGLOLabel(string label1, string label2, Rect r)
        {
            //https://docs.unity3d.com/ScriptReference/EditorGUILayout.LabelField.html
            //Shows a label in an editor window with the seconds since the editor started.
            /*
                public static void LabelField(string label, params GUILayoutOption[] options);
                public static void LabelField(string label, GUIStyle style, params GUILayoutOption[] options);
                public static void LabelField(GUIContent label, params GUILayoutOption[] options);
                public static void LabelField(GUIContent label, GUIStyle style, params GUILayoutOption[] options);
                public static void LabelField(string label, string label2, params GUILayoutOption[] options);
                public static void LabelField(string label, string label2, GUIStyle style, params GUILayoutOption[] options);
                public static void LabelField(GUIContent label, GUIContent label2, params GUILayoutOption[] options);
                public static void LabelField(GUIContent label, GUIContent label2, GUIStyle style, params GUILayoutOption[] options); 
             */

            EditorGUILayout.LabelField(label1, label2, guiTitleStyle, new GUILayoutOption[] {GUILayout.Width(50) });
        }
        #endregion editorguilo

        #region editorgui
        void DrawEGLabel(string label1, string label2, Rect r)
        {
            //https://docs.unity3d.com/ScriptReference/EditorGUI.LabelField.html
            //Shows a label in an editor window with the seconds since the editor started.
            /*
                public static void LabelField(Rect position, string label, GUIStyle style = EditorStyles.label);
                public static void LabelField(Rect position, GUIContent label, GUIStyle style = EditorStyles.label);
                public static void LabelField(Rect position, string label, string label2, GUIStyle style = EditorStyles.label);
                public static void LabelField(Rect position, GUIContent label, GUIContent label2, GUIStyle style = EditorStyles.label); 
             */

            EditorGUI.LabelField(r,label1, label2, guiTitleStyle);
        }

        void DrawEGTimeLabel()
        {
            EditorGUI.LabelField(new Rect(3, 3, position.width, 20),
           "Time since start: ",
           EditorApplication.timeSinceStartup.ToString());
            this.Repaint();
        }
        #endregion editorgui

        #region table
        void CreateRow(string[] rowData, int rowWidth, int rowHeight, float startX, float startY)
        {
            //  Rect lr = GUILayoutUtility.GetLastRect();
            Rect cr = GUILayoutUtility.GetRect(100, rowHeight );
            Rect r;
            for (int i = 0; i < rowData.Length; i++)
            {
                r = new Rect(startX + cr.x + i * rowWidth, cr.y, rowWidth, rowHeight);
                DrawEGLabel(rowData[i], "", r);
            }
        }

        void CreateTable(string[,] tableData, int rowWidth, float startX, float startY)
        {
            for (int i = 0; i < tableData.GetLength(0); i++)
            {
                string[] row = GetRow(tableData, i);
                CreateRow(row, rowWidth, 20, 0,0);
            }
        }

        public static GUIStyle guiTitleStyle
        {
            get
            {
                var guiTitleStyle = new GUIStyle(EditorStyles.whiteLabel);
                guiTitleStyle.normal.textColor = Color.red;
                guiTitleStyle.fontSize = 46;
                guiTitleStyle.fixedHeight = 30;
                guiTitleStyle.alignment = TextAnchor.MiddleCenter;
                return guiTitleStyle;
            }
        }
        #endregion table

        #region arrays utils
        public static T[] GetRow<T>(T[,] matrix, int row)
        {
            var columns = matrix.GetLength(1);
            var array = new T[columns];
            for (int i = 0; i < columns; ++i)
                array[i] = matrix[row, i];
            return array;
        }

        public static T[] GetColumn<T>(T[,] matrix, int column)
        {
            var rows = matrix.GetLength(0);
            var array = new T[rows];
            for (int i = 0; i < rows; ++i)
                array[i] = matrix[i, column];
            return array;
        }
        #endregion arrays utils

        #region asset utils
        /// <summary>
        /// Return array of paths to asset with name - assetName
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static string [] FindAssetPaths(string assetName)
        {
            https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
            var guids = AssetDatabase.FindAssets(assetName);
            if (guids == null || guids.Length == 0) return null;
            string[] res = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                res[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }
            return res;
        }

        /// <summary>
        /// Return first found texture with name - assetName
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static Texture FindFirsTexture(string assetName)
        {
           Texture tex;
           string []  paths =  FindAssetPaths(assetName);
           if (paths == null || paths.Length == 0) return null;
            for (int i = 0; i < paths.Length; i++)
            {
                tex = AssetDatabase.LoadAssetAtPath<Texture>(paths[i]);
                if (tex) return tex;
            }
            return null;
        }
        #endregion asset utils

        /*
        private void DropAreaGUI()
        {

            var e = Event.current.type;

            if (e == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
            else if (e == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                foreach (Object draggedObject in DragAndDrop.objectReferences)
                {
                    if (draggedObject is GameObject)
                    {
                        Debug.Log(draggedObject.name);
                    }
                }
            }

        }
        */

    }
}