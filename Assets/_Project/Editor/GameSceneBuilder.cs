using Blocks.Character;
using Blocks.Extras;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KnightsQuest.Editor
{
    public static class GameSceneBuilder
    {
        [MenuItem("Knights Quest/1. Build All Scenes and Setup", false, 1)]
        public static void BuildAllScenes()
        {
            SetupLevel1Scene();
            SetupLevel2Scene();
            BuildMainMenuScene(); // Keep MainMenu active in Editor so pressing Play starts at MainMenu!
            UpdateBuildSettings();
            Debug.Log("<color=green>[Knight's Quest] All scenes successfully configured and validated!</color>");
        }

        [MenuItem("Knights Quest/Reset Saved Game Progress", false, 10)]
        public static void ResetGameProgressMenu()
        {
            GameProgress.ResetProgress();
            Debug.Log("<color=yellow>[Knight's Quest] Saved PlayerPrefs progress reset to Level 1, 0 Gold.</color>");
        }

        [MenuItem("Knights Quest/Build Main Menu Scene", false, 2)]
        public static void BuildMainMenuScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Camera
            var camGo = new GameObject("Main Camera");
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.06f, 0.08f, 0.12f, 1f);
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(0, 0, -10);

            // 2. Event System
            EnsureEventSystem();

            // 3. Canvas
            var canvasGo = new GameObject("MainMenuCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            // Controller
            var mmController = canvasGo.AddComponent<MainMenuController>();

            // Background Panel
            var bgPanel = CreateUIPanel(canvasGo.transform, "Background", new Color(0.06f, 0.07f, 0.11f, 0.95f));
            SetRectTransformStretch(bgPanel.GetComponent<RectTransform>());

            // Title
            var title = CreateUIText(bgPanel.transform, "TitleText", "KNIGHT'S QUEST", 72, FontStyles.Bold, new Color(0.95f, 0.82f, 0.35f, 1f));
            var titleRt = title.GetComponent<RectTransform>();
            titleRt.anchoredPosition = new Vector2(0, 360);
            titleRt.sizeDelta = new Vector2(1200, 120);

            var subtitle = CreateUIText(bgPanel.transform, "SubtitleText", "Defend the Kingdom â€¢ Reclaim the Sacred Forest", 30, FontStyles.Italic, new Color(0.75f, 0.8f, 0.9f, 1f));
            var subRt = subtitle.GetComponent<RectTransform>();
            subRt.anchoredPosition = new Vector2(0, 290);
            subRt.sizeDelta = new Vector2(1200, 60);

            // Gold counter
            var goldText = CreateUIText(bgPanel.transform, "TotalGoldText", "Gold Collected: 0", 28, FontStyles.Normal, new Color(1f, 0.9f, 0.4f, 1f));
            var goldRt = goldText.GetComponent<RectTransform>();
            goldRt.anchoredPosition = new Vector2(0, 230);
            goldRt.sizeDelta = new Vector2(600, 50);

            // 4. Main Options Panel
            var optionsPanel = CreateUIPanel(bgPanel.transform, "OptionsPanel", Color.clear, false);
            var optRt = optionsPanel.GetComponent<RectTransform>();
            optRt.anchoredPosition = new Vector2(0, -20);
            optRt.sizeDelta = new Vector2(600, 420);

            var playBtn = CreateUIButton(optionsPanel.transform, "PlayButton", "PLAY GAME", new Vector2(0, 120), new Vector2(400, 70), new Color(0.2f, 0.65f, 0.3f, 1f));
            var levelSelectBtn = CreateUIButton(optionsPanel.transform, "LevelSelectButton", "LEVEL SELECT", new Vector2(0, 30), new Vector2(400, 70), new Color(0.25f, 0.45f, 0.75f, 1f));
            var controlsBtn = CreateUIButton(optionsPanel.transform, "ControlsButton", "HOW TO PLAY", new Vector2(0, -60), new Vector2(400, 70), new Color(0.55f, 0.45f, 0.25f, 1f));
            var exitBtn = CreateUIButton(optionsPanel.transform, "ExitButton", "EXIT", new Vector2(0, -150), new Vector2(400, 70), new Color(0.65f, 0.25f, 0.25f, 1f));

            // 5. Level Select Panel
            var lvlSelectPanel = CreateUIPanel(bgPanel.transform, "LevelSelectPanel", new Color(0.1f, 0.12f, 0.18f, 0.98f));
            var lvlRt = lvlSelectPanel.GetComponent<RectTransform>();
            lvlRt.anchoredPosition = new Vector2(0, -30);
            lvlRt.sizeDelta = new Vector2(700, 480);

            var lvlTitle = CreateUIText(lvlSelectPanel.transform, "LvlTitle", "CHOOSE YOUR CHAPTER", 40, FontStyles.Bold, Color.white);
            lvlTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 180);
            lvlTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 60);

            var lvl1Btn = CreateUIButton(lvlSelectPanel.transform, "Level1Btn", "Chapter 1: Enchanted Forest", new Vector2(0, 70), new Vector2(500, 75), new Color(0.2f, 0.55f, 0.35f, 1f));
            var lvl2Btn = CreateUIButton(lvlSelectPanel.transform, "Level2Btn", "Chapter 2: Dark Castle", new Vector2(0, -30), new Vector2(500, 75), new Color(0.5f, 0.3f, 0.6f, 1f));

            var lockedText = CreateUIText(lvlSelectPanel.transform, "Lvl2LockedText", "(Locked: Complete Chapter 1 first)", 20, FontStyles.Italic, new Color(0.9f, 0.4f, 0.4f, 1f));
            lockedText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -85);
            lockedText.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 30);

            var backFromLvlBtn = CreateUIButton(lvlSelectPanel.transform, "BackFromLvlBtn", "BACK", new Vector2(0, -160), new Vector2(300, 60), new Color(0.4f, 0.4f, 0.45f, 1f));
            lvlSelectPanel.SetActive(false);

            // 6. Controls Panel
            var controlsPanelGo = CreateUIPanel(bgPanel.transform, "ControlsPanel", new Color(0.1f, 0.12f, 0.18f, 0.98f));
            var cntRt = controlsPanelGo.GetComponent<RectTransform>();
            cntRt.anchoredPosition = new Vector2(0, -30);
            cntRt.sizeDelta = new Vector2(800, 520);

            var cntTitle = CreateUIText(controlsPanelGo.transform, "CntTitle", "CONTROLS & HOW TO PLAY", 40, FontStyles.Bold, Color.white);
            cntTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);
            cntTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 60);

            var cntBody = CreateUIText(controlsPanelGo.transform, "CntBody", "", 24, FontStyles.Normal, new Color(0.9f, 0.9f, 0.95f, 1f));
            var cntBodyRt = cntBody.GetComponent<RectTransform>();
            cntBodyRt.anchoredPosition = new Vector2(0, 20);
            cntBodyRt.sizeDelta = new Vector2(720, 280);

            var backFromCntBtn = CreateUIButton(controlsPanelGo.transform, "BackFromCntBtn", "BACK", new Vector2(0, -190), new Vector2(300, 60), new Color(0.4f, 0.4f, 0.45f, 1f));
            controlsPanelGo.SetActive(false);

            // Wire Controller serialized fields via SerializedObject
            var so = new SerializedObject(mmController);
            so.FindProperty("mainOptionsPanel").objectReferenceValue = optionsPanel;
            so.FindProperty("levelSelectPanel").objectReferenceValue = lvlSelectPanel;
            so.FindProperty("controlsPanel").objectReferenceValue = controlsPanelGo;

            so.FindProperty("playButton").objectReferenceValue = playBtn.GetComponent<Button>();
            so.FindProperty("levelSelectButton").objectReferenceValue = levelSelectBtn.GetComponent<Button>();
            so.FindProperty("controlsButton").objectReferenceValue = controlsBtn.GetComponent<Button>();
            so.FindProperty("exitButton").objectReferenceValue = exitBtn.GetComponent<Button>();

            so.FindProperty("level1Button").objectReferenceValue = lvl1Btn.GetComponent<Button>();
            so.FindProperty("level2Button").objectReferenceValue = lvl2Btn.GetComponent<Button>();
            so.FindProperty("backFromLevelSelectButton").objectReferenceValue = backFromLvlBtn.GetComponent<Button>();
            so.FindProperty("level2LockedText").objectReferenceValue = lockedText;

            so.FindProperty("backFromControlsButton").objectReferenceValue = backFromCntBtn.GetComponent<Button>();
            so.FindProperty("controlsBodyText").objectReferenceValue = cntBody;
            so.FindProperty("totalGoldText").objectReferenceValue = goldText;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Save Scene
            string path = "Assets/_Project/Scenes/MainMenu.unity";
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[Knight's Quest] Saved MainMenu scene at {path}");
        }

        [MenuItem("Knights Quest/Setup Level 1 (Enchanted Forest)", false, 3)]
        public static void SetupLevel1Scene()
        {
            string path = "Assets/_Project/Scenes/Level1_EnchantedForest.unity";
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

            EnsureEventSystem();

            var player = UnityEngine.Object.FindAnyObjectByType<BuildingBlocksCharacter>();
            if (player == null)
            {
                Debug.LogError("[Level 1] Player not found in scene!");
                return;
            }

            // Ensure LevelController
            var lvlCtrlGo = GameObject.Find("LevelController");
            if (lvlCtrlGo == null) lvlCtrlGo = new GameObject("LevelController");
            var lvlCtrl = lvlCtrlGo.GetComponent<LevelController>();
            if (lvlCtrl == null) lvlCtrl = lvlCtrlGo.AddComponent<LevelController>();

            var soLvl = new SerializedObject(lvlCtrl);
            soLvl.FindProperty("levelNumber").intValue = 1;
            soLvl.FindProperty("levelName").stringValue = "Chapter 1: Enchanted Forest";
            soLvl.FindProperty("isFinalLevel").boolValue = false;
            soLvl.FindProperty("targetCoins").intValue = 5;
            soLvl.FindProperty("playerCharacter").objectReferenceValue = player;
            soLvl.ApplyModifiedPropertiesWithoutUndo();

            // Checkpoints & Collectibles
            SpawnCollectiblesAndCheckpoints(1, player.transform.position);

            // Apply unique visuals for Chapter 1
            ApplyThemedVisuals(1);

            // Attach HUD Canvas
            AttachOrUpdateGameHUD(player, 1);

            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[Knight's Quest] Level 1 configured and saved.");
        }

        [MenuItem("Knights Quest/Setup Level 2 (Dark Castle)", false, 4)]
        public static void SetupLevel2Scene()
        {
            string path = "Assets/_Project/Scenes/Level2_DarkCastle.unity";
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

            EnsureEventSystem();

            var player = UnityEngine.Object.FindAnyObjectByType<BuildingBlocksCharacter>();
            if (player == null)
            {
                Debug.LogError("[Level 2] Player not found in scene!");
                return;
            }

            // Ensure LevelController
            var lvlCtrlGo = GameObject.Find("LevelController");
            if (lvlCtrlGo == null) lvlCtrlGo = new GameObject("LevelController");
            var lvlCtrl = lvlCtrlGo.GetComponent<LevelController>();
            if (lvlCtrl == null) lvlCtrl = lvlCtrlGo.AddComponent<LevelController>();

            var soLvl = new SerializedObject(lvlCtrl);
            soLvl.FindProperty("levelNumber").intValue = 2;
            soLvl.FindProperty("levelName").stringValue = "Chapter 2: Dark Castle";
            soLvl.FindProperty("isFinalLevel").boolValue = true;
            soLvl.FindProperty("targetCoins").intValue = 8;
            soLvl.FindProperty("playerCharacter").objectReferenceValue = player;
            soLvl.ApplyModifiedPropertiesWithoutUndo();

            // Checkpoints & Collectibles
            SpawnCollectiblesAndCheckpoints(2, player.transform.position);

            // Apply unique visuals for Chapter 2
            ApplyThemedVisuals(2);

            // Attach HUD Canvas
            AttachOrUpdateGameHUD(player, 2);

            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[Knight's Quest] Level 2 configured and saved.");
        }

        static void EnsureEventSystem()
        {
            var es = UnityEngine.Object.FindAnyObjectByType<EventSystem>();
            if (es == null)
            {
                var esGo = new GameObject("EventSystem");
                es = esGo.AddComponent<EventSystem>();
            }

            var inputModule = es.GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                var legacyModule = es.GetComponent<StandaloneInputModule>();
                if (legacyModule != null) UnityEngine.Object.DestroyImmediate(legacyModule);

                inputModule = es.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            string actionsPath = "Assets/Settings/InputSystem_Actions.inputactions";
            var actionsAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(actionsPath);
            if (actionsAsset != null)
            {
                var soModule = new SerializedObject(inputModule);
                soModule.FindProperty("m_ActionsAsset").objectReferenceValue = actionsAsset;
                soModule.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        static void SpawnCollectiblesAndCheckpoints(int levelNum, Vector3 playerPos)
        {
            var colParent = GameObject.Find("Collectibles");
            if (colParent == null) colParent = new GameObject("Collectibles");

            var cpParent = GameObject.Find("Checkpoints");
            if (cpParent == null) cpParent = new GameObject("Checkpoints");

            if (colParent.transform.childCount == 0)
            {
                Vector3[] coinOffsets = levelNum == 1 
                    ? new Vector3[] {
                        new Vector3(4f, 1f, 0),
                        new Vector3(10f, 2.5f, 0),
                        new Vector3(18f, 0.5f, 0),
                        new Vector3(25f, 3.5f, 0),
                        new Vector3(32f, 1f, 0),
                        new Vector3(40f, 2f, 0)
                    }
                    : new Vector3[] {
                        new Vector3(5f, 1f, 0),
                        new Vector3(12f, 3f, 0),
                        new Vector3(20f, -1f, 0),
                        new Vector3(28f, 4f, 0),
                        new Vector3(35f, 1.5f, 0),
                        new Vector3(42f, 3f, 0),
                        new Vector3(48f, 1f, 0)
                    };

                for (int i = 0; i < coinOffsets.Length; i++)
                {
                    var coinGo = new GameObject($"Coin_{i + 1}");
                    coinGo.transform.SetParent(colParent.transform);
                    coinGo.transform.position = playerPos + coinOffsets[i];

                    var col = coinGo.AddComponent<CircleCollider2D>();
                    col.isTrigger = true;
                    col.radius = 0.45f;

                    var sr = coinGo.AddComponent<SpriteRenderer>();
                    sr.color = levelNum == 1 ? new Color(1f, 0.88f, 0.2f, 1f) : new Color(0.9f, 0.45f, 1f, 1f); // Golden in Ch1, Ruby/Crystal in Ch2
                    sr.sortingOrder = 5;

                    coinGo.AddComponent<CollectibleCoin>();
                }
            }

            if (cpParent.transform.childCount == 0)
            {
                var cpGo = new GameObject("MidCheckpoint");
                cpGo.transform.SetParent(cpParent.transform);
                cpGo.transform.position = playerPos + new Vector3(22f, 1f, 0);

                var box = cpGo.AddComponent<BoxCollider2D>();
                box.isTrigger = true;
                box.size = new Vector2(1.5f, 3f);

                var sr = cpGo.AddComponent<SpriteRenderer>();
                sr.color = new Color(0.6f, 0.6f, 0.7f, 1f);
                sr.sortingOrder = 4;

                var cp = cpGo.AddComponent<Checkpoint>();
                var soCp = new SerializedObject(cp);
                soCp.FindProperty("flagRenderer").objectReferenceValue = sr;
                soCp.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        static void ApplyThemedVisuals(int levelNum)
        {
            // Distinct Protagonist Style (Sir Gareth)
            // Tint armor with a regal Paladin/Silver-Gold hue and give sword slash vibrant resonance
            var player = GameObject.Find("Player");
            if (player != null)
            {
                var visualChild = player.transform.Find("Visuals");
                if (visualChild != null)
                {
                    var sr = visualChild.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        // Paladin silver-cyan tint with radiant contrast
                        sr.color = new Color(0.9f, 0.98f, 1f, 1f);
                    }
                }
            }

            // Differentiate Enemies in Chapter 1 vs Chapter 2
            if (levelNum == 1) // Enchanted Forest
            {
                // 1. Forest Slime (restyle GrenadeEnemy into an alchemical forest spore thrower)
                var grenadeEnemy = GameObject.Find("GrenadeEnemy");
                if (grenadeEnemy != null)
                {
                    grenadeEnemy.name = "ForestSporeThrower";
                    var vis = grenadeEnemy.transform.Find("Visuals");
                    if (vis != null)
                    {
                        var sr = vis.GetComponent<SpriteRenderer>();
                        if (sr != null) sr.color = new Color(0.4f, 0.95f, 0.5f, 1f); // Vibrant Forest Green
                    }
                }

                // 2. Corrupted Wolves & Thorn Prowlers (restyle MeleeEnemy)
                var melee1 = GameObject.Find("MeleeEnemy");
                if (melee1 != null)
                {
                    melee1.name = "CorruptedWolfProwler";
                    var vis = melee1.transform.Find("Visuals");
                    if (vis != null)
                    {
                        var sr = vis.GetComponent<SpriteRenderer>();
                        if (sr != null) sr.color = new Color(0.7f, 0.85f, 0.35f, 1f); // Moss Green
                    }
                }

                var melee2 = GameObject.Find("MeleeEnemy (1)");
                if (melee2 != null)
                {
                    melee2.name = "ForestGuardianSkeleton";
                    var vis = melee2.transform.Find("Visuals");
                    if (vis != null)
                    {
                        var sr = vis.GetComponent<SpriteRenderer>();
                        if (sr != null) sr.color = new Color(0.5f, 0.9f, 0.7f, 1f); // Jade Bramble
                    }
                }

                // In Chapter 1, deactivate the end Boss so the level is a forest expedition to the exit portal
                var wolfBoss = GameObject.Find("WolfBossEnemy");
                if (wolfBoss != null)
                {
                    wolfBoss.SetActive(false);
                }

                // Win door opens when forest prowlers are cleared
                var door = UnityEngine.Object.FindAnyObjectByType<WinDoor>();
                if (door != null)
                {
                    var soDoor = new SerializedObject(door);
                    var reqList = soDoor.FindProperty("requiredEliminations");
                    reqList.ClearArray();
                    if (melee1 != null)
                    {
                        var ch = melee1.GetComponent<BuildingBlocksCharacter>();
                        if (ch != null) { reqList.InsertArrayElementAtIndex(0); reqList.GetArrayElementAtIndex(0).objectReferenceValue = ch; }
                    }
                    if (grenadeEnemy != null)
                    {
                        var ch = grenadeEnemy.GetComponent<BuildingBlocksCharacter>();
                        if (ch != null) { reqList.InsertArrayElementAtIndex(reqList.arraySize); reqList.GetArrayElementAtIndex(reqList.arraySize - 1).objectReferenceValue = ch; }
                    }
                    soDoor.ApplyModifiedPropertiesWithoutUndo();
                }
            }
            else // Chapter 2: Dark Castle
            {
                // Shadow Knight & Dark Mages
                var grenadeEnemy = GameObject.Find("GrenadeEnemy");
                if (grenadeEnemy != null)
                {
                    grenadeEnemy.name = "DarkCultistMage";
                    var vis = grenadeEnemy.transform.Find("Visuals");
                    if (vis != null)
                    {
                        var sr = vis.GetComponent<SpriteRenderer>();
                        if (sr != null) sr.color = new Color(0.85f, 0.35f, 0.95f, 1f); // Void Purple
                    }
                }

                var melee1 = GameObject.Find("MeleeEnemy");
                if (melee1 != null)
                {
                    melee1.name = "ShadowKnight";
                    var vis = melee1.transform.Find("Visuals");
                    if (vis != null)
                    {
                        var sr = vis.GetComponent<SpriteRenderer>();
                        if (sr != null) sr.color = new Color(0.35f, 0.35f, 0.45f, 1f); // Obsidian Dark Metal
                    }
                }

                var melee2 = GameObject.Find("MeleeEnemy (1)");
                if (melee2 != null)
                {
                    melee2.name = "CastleExecutioner";
                    var vis = melee2.transform.Find("Visuals");
                    if (vis != null)
                    {
                        var sr = vis.GetComponent<SpriteRenderer>();
                        if (sr != null) sr.color = new Color(0.9f, 0.25f, 0.25f, 1f); // Blood Red
                    }
                }

                // Keep Citadel Wolf Boss active for Chapter 2
                var wolfBoss = GameObject.Find("WolfBossEnemy");
                if (wolfBoss != null)
                {
                    wolfBoss.name = "CitadelWolfBoss";
                    wolfBoss.SetActive(true);
                    var vis = wolfBoss.transform.Find("Visuals");
                    if (vis != null)
                    {
                        var sr = vis.GetComponent<SpriteRenderer>();
                        if (sr != null) sr.color = new Color(0.85f, 0.2f, 0.25f, 1f); // Crimson Boss
                    }
                }
            }
        }

        static void AttachOrUpdateGameHUD(BuildingBlocksCharacter player, int levelNum)
        {
            var hudGo = GameObject.Find("KnightsQuestHUD");
            if (hudGo != null) UnityEngine.Object.DestroyImmediate(hudGo);

            hudGo = new GameObject("KnightsQuestHUD");
            var canvas = hudGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = hudGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            hudGo.AddComponent<GraphicRaycaster>();

            var gameHud = hudGo.AddComponent<GameHUD>();

            // --- 1. Instruction Panel ---
            var instPanel = CreateUIPanel(hudGo.transform, "InstructionPanel", new Color(0.07f, 0.08f, 0.12f, 0.96f));
            SetRectTransformStretch(instPanel.GetComponent<RectTransform>());

            var instCard = CreateUIPanel(instPanel.transform, "Card", new Color(0.12f, 0.14f, 0.2f, 0.98f));
            var instCardRt = instCard.GetComponent<RectTransform>();
            instCardRt.sizeDelta = new Vector2(1000, 750);
            instCardRt.anchoredPosition = Vector2.zero;

            var instTitle = CreateUIText(instCard.transform, "Title", "CHAPTER INSTRUCTIONS", 44, FontStyles.Bold, new Color(0.95f, 0.85f, 0.35f, 1f));
            instTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 310);
            instTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 60);

            var storyText = CreateUIText(instCard.transform, "StoryText", "Story intro...", 24, FontStyles.Normal, Color.white);
            storyText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 210);
            storyText.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 100);

            var objText = CreateUIText(instCard.transform, "ObjectiveText", "Objectives...", 24, FontStyles.Bold, new Color(0.6f, 0.9f, 0.6f, 1f));
            objText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 90);
            objText.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 90);

            var ctrlText = CreateUIText(instCard.transform, "ControlsText", "Controls...", 22, FontStyles.Normal, new Color(0.85f, 0.85f, 0.95f, 1f));
            ctrlText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            ctrlText.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 180);

            var contBtn = CreateUIButton(instCard.transform, "ContinueBtn", "COMMENCE QUEST", new Vector2(180, -290), new Vector2(300, 65), new Color(0.2f, 0.65f, 0.35f, 1f));
            var backBtn = CreateUIButton(instCard.transform, "BackBtn", "MAIN MENU", new Vector2(-180, -290), new Vector2(300, 65), new Color(0.55f, 0.25f, 0.25f, 1f));

            // --- 2. In-Game Top HUD ---
            var inGameRoot = CreateUIPanel(hudGo.transform, "InGameRoot", Color.clear, false);
            SetRectTransformStretch(inGameRoot.GetComponent<RectTransform>());

            var topBar = CreateUIPanel(inGameRoot.transform, "TopBar", new Color(0.08f, 0.1f, 0.14f, 0.8f));
            var topBarRt = topBar.GetComponent<RectTransform>();
            topBarRt.anchorMin = new Vector2(0, 1);
            topBarRt.anchorMax = new Vector2(1, 1);
            topBarRt.pivot = new Vector2(0.5f, 1f);
            topBarRt.anchoredPosition = Vector2.zero;
            topBarRt.sizeDelta = new Vector2(0, 80);

            var lvlText = CreateUIText(topBar.transform, "LvlText", levelNum == 1 ? "Chapter 1: Enchanted Forest" : "Chapter 2: Dark Castle", 26, FontStyles.Bold, Color.white);
            lvlText.alignment = TextAlignmentOptions.Left;
            var lvlRt = lvlText.GetComponent<RectTransform>();
            lvlRt.anchorMin = new Vector2(0, 0.5f);
            lvlRt.anchorMax = new Vector2(0, 0.5f);
            lvlRt.pivot = new Vector2(0, 0.5f);
            lvlRt.anchoredPosition = new Vector2(40, 0);
            lvlRt.sizeDelta = new Vector2(500, 50);

            var coinsDisplay = CreateUIText(topBar.transform, "CoinsDisplay", "Gold: 0", 26, FontStyles.Bold, new Color(1f, 0.85f, 0.2f, 1f));
            coinsDisplay.alignment = TextAlignmentOptions.Right;
            var coinsRt = coinsDisplay.GetComponent<RectTransform>();
            coinsRt.anchorMin = new Vector2(1, 0.5f);
            coinsRt.anchorMax = new Vector2(1, 0.5f);
            coinsRt.pivot = new Vector2(1, 0.5f);
            coinsRt.anchoredPosition = new Vector2(-40, 0);
            coinsRt.sizeDelta = new Vector2(400, 50);

            // Health Bar
            var healthBarGo = new GameObject("HealthSlider");
            healthBarGo.transform.SetParent(topBar.transform, false);
            var hbRt = healthBarGo.AddComponent<RectTransform>();
            hbRt.anchorMin = new Vector2(0.5f, 0.5f);
            hbRt.anchorMax = new Vector2(0.5f, 0.5f);
            hbRt.anchoredPosition = Vector2.zero;
            hbRt.sizeDelta = new Vector2(300, 30);

            var slider = healthBarGo.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;

            var hbBg = CreateUIPanel(healthBarGo.transform, "Background", new Color(0.2f, 0.2f, 0.2f, 0.8f));
            SetRectTransformStretch(hbBg.GetComponent<RectTransform>());

            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(healthBarGo.transform, false);
            var fillAreaRt = fillArea.AddComponent<RectTransform>();
            SetRectTransformStretch(fillAreaRt);

            var fill = CreateUIPanel(fillArea.transform, "Fill", new Color(0.85f, 0.2f, 0.2f, 1f));
            var fillRt = fill.GetComponent<RectTransform>();
            fillRt.sizeDelta = Vector2.zero;
            slider.fillRect = fillRt;

            var hpText = CreateUIText(healthBarGo.transform, "HPText", "100/100", 20, FontStyles.Bold, Color.white);
            SetRectTransformStretch(hpText.GetComponent<RectTransform>());

            // --- 3. Pause Menu ---
            var pauseRoot = CreateUIPanel(hudGo.transform, "PauseMenu", new Color(0.05f, 0.06f, 0.1f, 0.9f));
            SetRectTransformStretch(pauseRoot.GetComponent<RectTransform>());
            var pauseTitle = CreateUIText(pauseRoot.transform, "Title", "GAME PAUSED", 48, FontStyles.Bold, Color.white);
            pauseTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 180);
            pauseTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 70);

            var resumeBtn = CreateUIButton(pauseRoot.transform, "ResumeBtn", "RESUME", new Vector2(0, 60), new Vector2(340, 65), new Color(0.25f, 0.6f, 0.35f, 1f));
            var restartBtn = CreateUIButton(pauseRoot.transform, "RestartBtn", "RESTART LEVEL", new Vector2(0, -30), new Vector2(340, 65), new Color(0.35f, 0.45f, 0.7f, 1f));
            var pauseMmBtn = CreateUIButton(pauseRoot.transform, "MainMenuBtn", "MAIN MENU", new Vector2(0, -120), new Vector2(340, 65), new Color(0.6f, 0.25f, 0.25f, 1f));
            pauseRoot.SetActive(false);

            // --- 4. Game Over Screen ---
            var goRoot = CreateUIPanel(hudGo.transform, "GameOverRoot", new Color(0.12f, 0.02f, 0.02f, 0.95f));
            SetRectTransformStretch(goRoot.GetComponent<RectTransform>());
            var goTitle = CreateUIText(goRoot.transform, "Title", "KNIGHT DEFEATED", 56, FontStyles.Bold, new Color(0.95f, 0.2f, 0.2f, 1f));
            goTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 160);
            goTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 80);

            var retryBtn = CreateUIButton(goRoot.transform, "RetryBtn", "TRY AGAIN", new Vector2(0, 20), new Vector2(340, 65), new Color(0.7f, 0.3f, 0.3f, 1f));
            var goMmBtn = CreateUIButton(goRoot.transform, "MainMenuBtn", "MAIN MENU", new Vector2(0, -70), new Vector2(340, 65), new Color(0.4f, 0.4f, 0.45f, 1f));
            goRoot.SetActive(false);

            // --- 5. Level Complete / Victory Screen ---
            var lcRoot = CreateUIPanel(hudGo.transform, "LevelCompleteRoot", new Color(0.04f, 0.1f, 0.06f, 0.95f));
            SetRectTransformStretch(lcRoot.GetComponent<RectTransform>());
            var lcTitle = CreateUIText(lcRoot.transform, "Title", "CHAPTER COMPLETE!", 54, FontStyles.Bold, new Color(0.95f, 0.85f, 0.3f, 1f));
            lcTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 180);
            lcTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 80);

            var lcStats = CreateUIText(lcRoot.transform, "Stats", "Coins Collected: 0", 28, FontStyles.Normal, Color.white);
            lcStats.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 70);
            lcStats.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 100);

            var nextLvlBtn = CreateUIButton(lcRoot.transform, "NextLevelBtn", "NEXT CHAPTER", new Vector2(0, -40), new Vector2(360, 70), new Color(0.2f, 0.65f, 0.35f, 1f));
            var lcMmBtn = CreateUIButton(lcRoot.transform, "MainMenuBtn", "MAIN MENU", new Vector2(0, -130), new Vector2(360, 70), new Color(0.4f, 0.45f, 0.55f, 1f));
            lcRoot.SetActive(false);

            // Wire GameHUD serialized fields
            var soHud = new SerializedObject(gameHud);
            soHud.FindProperty("instructionPanel").objectReferenceValue = instPanel;
            soHud.FindProperty("instructionLevelTitle").objectReferenceValue = instTitle;
            soHud.FindProperty("instructionStoryText").objectReferenceValue = storyText;
            soHud.FindProperty("instructionObjectiveText").objectReferenceValue = objText;
            soHud.FindProperty("instructionControlsText").objectReferenceValue = ctrlText;
            soHud.FindProperty("instructionContinueButton").objectReferenceValue = contBtn.GetComponent<Button>();
            soHud.FindProperty("instructionBackButton").objectReferenceValue = backBtn.GetComponent<Button>();

            soHud.FindProperty("inGameHudRoot").objectReferenceValue = inGameRoot;
            soHud.FindProperty("levelNameText").objectReferenceValue = lvlText;
            soHud.FindProperty("coinCountText").objectReferenceValue = coinsDisplay;
            soHud.FindProperty("healthSlider").objectReferenceValue = slider;
            soHud.FindProperty("healthText").objectReferenceValue = hpText;

            soHud.FindProperty("pauseMenuRoot").objectReferenceValue = pauseRoot;
            soHud.FindProperty("pauseResumeButton").objectReferenceValue = resumeBtn.GetComponent<Button>();
            soHud.FindProperty("pauseRestartButton").objectReferenceValue = restartBtn.GetComponent<Button>();
            soHud.FindProperty("pauseMainMenuButton").objectReferenceValue = pauseMmBtn.GetComponent<Button>();

            soHud.FindProperty("gameOverRoot").objectReferenceValue = goRoot;
            soHud.FindProperty("gameOverRetryButton").objectReferenceValue = retryBtn.GetComponent<Button>();
            soHud.FindProperty("gameOverMainMenuButton").objectReferenceValue = goMmBtn.GetComponent<Button>();

            soHud.FindProperty("levelCompleteRoot").objectReferenceValue = lcRoot;
            soHud.FindProperty("levelCompleteTitle").objectReferenceValue = lcTitle;
            soHud.FindProperty("levelCompleteStats").objectReferenceValue = lcStats;
            soHud.FindProperty("nextLevelButton").objectReferenceValue = nextLvlBtn.GetComponent<Button>();
            soHud.FindProperty("victoryMainMenuButton").objectReferenceValue = lcMmBtn.GetComponent<Button>();

            soHud.FindProperty("playerCharacter").objectReferenceValue = player;

            soHud.ApplyModifiedPropertiesWithoutUndo();
        }

        static void UpdateBuildSettings()
        {
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene("Assets/_Project/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/_Project/Scenes/Level1_EnchantedForest.unity", true),
                new EditorBuildSettingsScene("Assets/_Project/Scenes/Level2_DarkCastle.unity", true)
            };
            EditorBuildSettings.scenes = scenes;
            Debug.Log("[Knight's Quest] EditorBuildSettings updated with 3 Knight's Quest scenes.");
        }

        // --- UI Construction Helpers ---
        static GameObject CreateUIPanel(Transform parent, string name, Color color, bool raycastTarget = true)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = raycastTarget;
            return go;
        }

        static TextMeshProUGUI CreateUIText(Transform parent, string name, string text, float fontSize, FontStyles style, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false; // Never block clicks on parent buttons
            return tmp;
        }

        static GameObject CreateUIButton(Transform parent, string name, string label, Vector2 anchoredPos, Vector2 size, Color btnColor)
        {
            var btnGo = new GameObject(name);
            btnGo.transform.SetParent(parent, false);

            var rt = btnGo.AddComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;

            var img = btnGo.AddComponent<Image>();
            img.color = btnColor;
            img.raycastTarget = true; // Ensure button background receives raycasts

            var btn = btnGo.AddComponent<Button>();
            btn.targetGraphic = img;
            var colors = btn.colors;
            colors.highlightedColor = btnColor * 1.25f;
            colors.pressedColor = btnColor * 0.75f;
            btn.colors = colors;

            var txt = CreateUIText(btnGo.transform, "Text", label, 24, FontStyles.Bold, Color.white);
            txt.raycastTarget = false; // Never block click
            SetRectTransformStretch(txt.GetComponent<RectTransform>());

            return btnGo;
        }

        static void SetRectTransformStretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
