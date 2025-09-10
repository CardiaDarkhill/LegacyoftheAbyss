using System;
using GlobalEnums;
using InControl;

// Token: 0x02000757 RID: 1879
public static class Constants
{
	// Token: 0x060042A8 RID: 17064 RVA: 0x00125D31 File Offset: 0x00123F31
	public static T GetConstantValue<T>(string variableName)
	{
		return Constants.fieldCache.GetValue<T>(variableName);
	}

	// Token: 0x04004425 RID: 17445
	public const string GAME_VERSION = "1.0.28324";

	// Token: 0x04004426 RID: 17446
	public const int SILK_PARTS_COUNT = 3;

	// Token: 0x04004427 RID: 17447
	public const int BIND_SILK_COST = 9;

	// Token: 0x04004428 RID: 17448
	public const int BIND_SILK_COST_WITCH = 9;

	// Token: 0x04004429 RID: 17449
	public const int MAX_SILK = 18;

	// Token: 0x0400442A RID: 17450
	public const int SPOOL_PIECES_PER_SILK = 2;

	// Token: 0x0400442B RID: 17451
	public const int MAX_SILK_SPOOLS = 18;

	// Token: 0x0400442C RID: 17452
	public const int SILK_SKILL_COST = 4;

	// Token: 0x0400442D RID: 17453
	public const int SILK_SKILL_COST_FLEACHARM = 3;

	// Token: 0x0400442E RID: 17454
	public const int STARTING_HEALTH = 5;

	// Token: 0x0400442F RID: 17455
	public const int MAX_HEALTH = 10;

	// Token: 0x04004430 RID: 17456
	public const int HEART_PIECES_PER_HEALTH = 4;

	// Token: 0x04004431 RID: 17457
	public const int MAX_HEART_PIECES = 20;

	// Token: 0x04004432 RID: 17458
	public const int BLUE_HEALTH_MID = 5;

	// Token: 0x04004433 RID: 17459
	public const int BLUE_HEALTH_FINAL = 7;

	// Token: 0x04004434 RID: 17460
	public const int MAX_BLUE_HEALTH = 9;

	// Token: 0x04004435 RID: 17461
	public const int MAX_SILK_REGEN = 3;

	// Token: 0x04004436 RID: 17462
	public const int MAP_MARKER_COUNT = 9;

	// Token: 0x04004437 RID: 17463
	public const int FLEA_FESTIVAL_CHAMP_JUGGLE = 30;

	// Token: 0x04004438 RID: 17464
	public const int FLEA_FESTIVAL_SETH_JUGGLE = 55;

	// Token: 0x04004439 RID: 17465
	public const int FLEA_FESTIVAL_CHAMP_DODGE = 65;

	// Token: 0x0400443A RID: 17466
	public const int FLEA_FESTIVAL_SETH_DODGE = 95;

	// Token: 0x0400443B RID: 17467
	public const int FLEA_FESTIVAL_CHAMP_BOUNCE = 42;

	// Token: 0x0400443C RID: 17468
	public const int FLEA_FESTIVAL_SETH_BOUNCE = 68;

	// Token: 0x0400443D RID: 17469
	public const float CORAL_SPEAR_SPAWN_TIME = 2f;

	// Token: 0x0400443E RID: 17470
	public const int MAGGOT_CHARM_MAX_HITS = 3;

	// Token: 0x0400443F RID: 17471
	public const float DEFAULT_TIMESCALE = 1f;

	// Token: 0x04004440 RID: 17472
	public const float PAUSED_TIMESCALE = 0f;

	// Token: 0x04004441 RID: 17473
	public const float FRAME_WAIT = 0.165f;

	// Token: 0x04004442 RID: 17474
	public const float TIME_SCALE_CHANGE_RATE = 1E-05f;

	// Token: 0x04004443 RID: 17475
	public const float SCENE_TRANSITION_WAIT = 0.34f;

	// Token: 0x04004444 RID: 17476
	public const float SCENE_TRANSITION_FADE = 0.25f;

	// Token: 0x04004445 RID: 17477
	public const float HERO_DEFAULT_GRAVITY = 0.79f;

	// Token: 0x04004446 RID: 17478
	public const float HERO_UNDERWATER_GRAVITY = 0.225f;

	// Token: 0x04004447 RID: 17479
	public const float RAYCAST_EXTENTS = 0.16f;

	// Token: 0x04004448 RID: 17480
	public const float MIN_WALL_HEIGHT = 0.2f;

	// Token: 0x04004449 RID: 17481
	public const float INPUT_LOWER_SNAP_V = 0.5f;

	// Token: 0x0400444A RID: 17482
	public const float INPUT_LOWER_SNAP_H = 0.3f;

	// Token: 0x0400444B RID: 17483
	public const float INPUT_UPPER_SNAP = 0.9f;

	// Token: 0x0400444C RID: 17484
	public const float INPUT_DEADZONE_L = 0.15f;

	// Token: 0x0400444D RID: 17485
	public const float INPUT_DEADZONE_U = 0.95f;

	// Token: 0x0400444E RID: 17486
	public const float CAM_Z_DEFAULT = -38.1f;

	// Token: 0x0400444F RID: 17487
	public const float CAM_BOUND_X = 14.6f;

	// Token: 0x04004450 RID: 17488
	public const float CAM_BOUND_Y = 8.3f;

	// Token: 0x04004451 RID: 17489
	public const float CAM_HOR_OFFSET_AMOUNT = 1f;

	// Token: 0x04004452 RID: 17490
	public const float CAM_FALL_VELOCITY = -20f;

	// Token: 0x04004453 RID: 17491
	public const float CAM_FALL_OFFSET = -4f;

	// Token: 0x04004454 RID: 17492
	public const float CAM_LOOK_OFFSET = 6f;

	// Token: 0x04004455 RID: 17493
	public const float CAM_START_LOCKED_TIMER = 0.65f;

	// Token: 0x04004456 RID: 17494
	public const float CAM_HAZARD_RESPAWN_FROZEN = 0.5f;

	// Token: 0x04004457 RID: 17495
	public const float CAM_MENU_X = 14.6f;

	// Token: 0x04004458 RID: 17496
	public const float CAM_MENU_Y = 8.5f;

	// Token: 0x04004459 RID: 17497
	public const float CAM_CIN_X = 14.6f;

	// Token: 0x0400445A RID: 17498
	public const float CAM_CIN_Y = 8.5f;

	// Token: 0x0400445B RID: 17499
	public const float CAM_CUT_X = 14.6f;

	// Token: 0x0400445C RID: 17500
	public const float CAM_CUT_Y = 8.5f;

	// Token: 0x0400445D RID: 17501
	public const float CAM_STAG_PRE_FADEOUT = 0.6f;

	// Token: 0x0400445E RID: 17502
	public const float CAM_FADE_TIME_START_FADE = 2.3f;

	// Token: 0x0400445F RID: 17503
	public const float CAM_DEFAULT_BLUR_DEPTH = 6.62f;

	// Token: 0x04004460 RID: 17504
	public const float CAM_DEFAULT_SATURATION = 0.7f;

	// Token: 0x04004461 RID: 17505
	public const float CAM_DEFAULT_INTENSITY = 0.7f;

	// Token: 0x04004462 RID: 17506
	public const float MIN_VIEW_DEPTH = 10f;

	// Token: 0x04004463 RID: 17507
	public const float MAX_VIEW_DEPTH = 1000f;

	// Token: 0x04004464 RID: 17508
	public const float CAM_OVERLAP = 1E-05f;

	// Token: 0x04004465 RID: 17509
	public const float CAM_ORTHOSIZE = 8.710664f;

	// Token: 0x04004466 RID: 17510
	public const float CAM_CANVAS_MOVE_WAIT = 0.5f;

	// Token: 0x04004467 RID: 17511
	public const float CINEMATIC_SKIP_FADE_TIME = 0.3f;

	// Token: 0x04004468 RID: 17512
	public const float CAM_GAME_ASPECT_REF = 1.7777778f;

	// Token: 0x04004469 RID: 17513
	public const float CAM_GAME_ASPECT_WIDE = 2.3916667f;

	// Token: 0x0400446A RID: 17514
	public const float CAM_GAME_ASPECT_TALL = 1.6f;

	// Token: 0x0400446B RID: 17515
	public const string CAM_SHAKE_ENEMYKILL = "EnemyKillShake";

	// Token: 0x0400446C RID: 17516
	public const float SCENE_POSITION_LIMIT = 60f;

	// Token: 0x0400446D RID: 17517
	public const string MENU_SCENE = "Menu_Title";

	// Token: 0x0400446E RID: 17518
	public const string FIRST_LEVEL_NAME = "Tut_01";

	// Token: 0x0400446F RID: 17519
	public const string FIRST_LEVEL_RESPAWN_POINT = "Death Respawn Marker Init";

	// Token: 0x04004470 RID: 17520
	public const string STARTING_SCENE = "Opening_Sequence";

	// Token: 0x04004471 RID: 17521
	public const string INTRO_PROLOGUE = "Intro_Cutscene_Prologue";

	// Token: 0x04004472 RID: 17522
	public const string OPENING_CUTSCENE = "Intro_Cutscene";

	// Token: 0x04004473 RID: 17523
	public const string STAG_CINEMATIC = "Cinematic_Stag_travel";

	// Token: 0x04004474 RID: 17524
	public const string PERMADEATH_LEVEL = "PermaDeath";

	// Token: 0x04004475 RID: 17525
	public const string PERMADEATH_UNLOCK = "PermaDeath_Unlock";

	// Token: 0x04004476 RID: 17526
	public const string MRMUSHROOM_CINEMATIC = "Cinematic_MrMushroom";

	// Token: 0x04004477 RID: 17527
	public const string ENDING_A_CINEMATIC = "Cinematic_Ending_A";

	// Token: 0x04004478 RID: 17528
	public const string ENDING_B_CINEMATIC = "Cinematic_Ending_B";

	// Token: 0x04004479 RID: 17529
	public const string ENDING_C_CINEMATIC = "Cinematic_Ending_C";

	// Token: 0x0400447A RID: 17530
	public const string ENDING_D_CINEMATIC = "Cinematic_Ending_D";

	// Token: 0x0400447B RID: 17531
	public const string ENDING_E_CINEMATIC = "Cinematic_Ending_E";

	// Token: 0x0400447C RID: 17532
	public const string END_CREDITS = "End_Credits";

	// Token: 0x0400447D RID: 17533
	public const string END_CREDITS_SCROLL = "End_Credits_Scroll";

	// Token: 0x0400447E RID: 17534
	public const string MENU_CREDITS = "Menu_Credits";

	// Token: 0x0400447F RID: 17535
	public const string TITLE_SCREEN_LEVEL = "Title_Screens";

	// Token: 0x04004480 RID: 17536
	public const string TUTORIAL_LEVEL = "Tutorial_01";

	// Token: 0x04004481 RID: 17537
	public const string BOSS_DOOR_CUTSCENE = "Cutscene_Boss_Door";

	// Token: 0x04004482 RID: 17538
	public const string GAME_COMPLETION_SCREEN = "End_Game_Completion";

	// Token: 0x04004483 RID: 17539
	public const string BOSSRUSH_END_SCENE = "GG_End_Sequence";

	// Token: 0x04004484 RID: 17540
	public const string GG_ENTRANCE_SCENE = "GG_Entrance_Cutscene";

	// Token: 0x04004485 RID: 17541
	public const string GG_DOOR_ENTRANCE_SCENE = "GG_Boss_Door_Entrance";

	// Token: 0x04004486 RID: 17542
	public const string GG_RETURN_SCENE = "GG_Waterways";

	// Token: 0x04004487 RID: 17543
	public const string DUST_MAZE_ENTRANCE_SCENE = "Dust_Maze_09_entrance";

	// Token: 0x04004488 RID: 17544
	public const string SAVE_ICON_START_EVENT = "GAME SAVING";

	// Token: 0x04004489 RID: 17545
	public const string SAVE_ICON_END_EVENT = "GAME SAVED";

	// Token: 0x0400448A RID: 17546
	public const float HERO_Z = 0.004f;

	// Token: 0x0400448B RID: 17547
	public const float HAZARD_DEATH_WAIT = 0f;

	// Token: 0x0400448C RID: 17548
	public const float RESPAWN_FADEOUT_WAIT = 0.65f;

	// Token: 0x0400448D RID: 17549
	public const float HAZ_RESPAWN_FADEIN_WAIT = 0.1f;

	// Token: 0x0400448E RID: 17550
	public const float SCENE_ENTER_WAIT = 0.33f;

	// Token: 0x0400448F RID: 17551
	public const float QUICKENING_PITCH_OFFSET = 0.05f;

	// Token: 0x04004490 RID: 17552
	public const int MID_SHARD_VALUE = 5;

	// Token: 0x04004491 RID: 17553
	public const float CAMERA_MARGIN_X = 15f;

	// Token: 0x04004492 RID: 17554
	public const float CAMERA_MARGIN_Y = 9f;

	// Token: 0x04004493 RID: 17555
	public const float CUTSCENE_PROMPT_TIMEOUT = 3f;

	// Token: 0x04004494 RID: 17556
	public const float CUTSCENE_PROMPT_SKIP_COOLDOWN = 0.3f;

	// Token: 0x04004495 RID: 17557
	public const float SAVE_FLEUR_PAUSE = 0.1f;

	// Token: 0x04004496 RID: 17558
	public const float AREA_TITLE_UI_MSG_Y = -4f;

	// Token: 0x04004497 RID: 17559
	public const string RECORD_PERMADEATH_MODE = "RecPermadeathMode";

	// Token: 0x04004498 RID: 17560
	public const string RECORD_BOSSRUSH_MODE = "RecBossRushMode";

	// Token: 0x04004499 RID: 17561
	public const SupportedLanguages DEFAULT_LANGUAGE = SupportedLanguages.EN;

	// Token: 0x0400449A RID: 17562
	public const int DEFAULT_BACKERCREDITS = 0;

	// Token: 0x0400449B RID: 17563
	public const int DEFAULT_NATIVEPOPUPS = 0;

	// Token: 0x0400449C RID: 17564
	public const float MM_AUDIO_MASTER_VOL = 10f;

	// Token: 0x0400449D RID: 17565
	public const float MM_AUDIO_MUSIC_VOL = 10f;

	// Token: 0x0400449E RID: 17566
	public const float MM_AUDIO_SOUND_VOL = 10f;

	// Token: 0x0400449F RID: 17567
	public const int MM_VIDEO_RESX = 1920;

	// Token: 0x040044A0 RID: 17568
	public const int MM_VIDEO_RESY = 1080;

	// Token: 0x040044A1 RID: 17569
	public const int MM_VIDEO_FULLSCREEN = 2;

	// Token: 0x040044A2 RID: 17570
	public const int MM_VIDEO_VSYNC = 1;

	// Token: 0x040044A3 RID: 17571
	public const int DEFAULT_TARGET_FRAME_RATE = 60;

	// Token: 0x040044A4 RID: 17572
	public const int DEFAULT_DISPLAY = 0;

	// Token: 0x040044A5 RID: 17573
	public const int DEFAULT_VIDEO_PARTICLES = 1;

	// Token: 0x040044A6 RID: 17574
	public const ShaderQualities DEFAULT_VIDEO_SHADER_QUALITY = ShaderQualities.High;

	// Token: 0x040044A7 RID: 17575
	public const float MM_OS_MAINCAM = 1f;

	// Token: 0x040044A8 RID: 17576
	public const float MM_OS_HUDCAM = 8.710664f;

	// Token: 0x040044A9 RID: 17577
	public const float MM_OS_DEFAULT = 0f;

	// Token: 0x040044AA RID: 17578
	public const float DEFAULT_BRIGHTNESS = 20f;

	// Token: 0x040044AB RID: 17579
	public const GamepadType MM_INPUTTYPE = GamepadType.NONE;

	// Token: 0x040044AC RID: 17580
	public const ControllerProfile MM_INPUTPROFILE = ControllerProfile.Default;

	// Token: 0x040044AD RID: 17581
	public const Key DEFAULT_KEY_JUMP = Key.Z;

	// Token: 0x040044AE RID: 17582
	public const Key DEFAULT_KEY_ATTACK = Key.X;

	// Token: 0x040044AF RID: 17583
	public const Key DEFAULT_KEY_DASH = Key.C;

	// Token: 0x040044B0 RID: 17584
	public const Key DEFAULT_KEY_CAST = Key.A;

	// Token: 0x040044B1 RID: 17585
	public const Key DEFAULT_KEY_SUPERDASH = Key.S;

	// Token: 0x040044B2 RID: 17586
	public const Key DEFAULT_KEY_DREAMNAIL = Key.D;

	// Token: 0x040044B3 RID: 17587
	public const Key DEFAULT_KEY_QUICKMAP = Key.Tab;

	// Token: 0x040044B4 RID: 17588
	public const Key DEFAULT_KEY_QUICKCAST = Key.F;

	// Token: 0x040044B5 RID: 17589
	public const Key DEFAULT_KEY_INVENTORY = Key.I;

	// Token: 0x040044B6 RID: 17590
	public const Key DEFAULT_KEY_INVENTORY_MAP = Key.M;

	// Token: 0x040044B7 RID: 17591
	public const Key DEFAULT_KEY_INVENTORY_JOURNAL = Key.J;

	// Token: 0x040044B8 RID: 17592
	public const Key DEFAULT_KEY_INVENTORY_TOOLS = Key.Q;

	// Token: 0x040044B9 RID: 17593
	public const Key DEFAULT_KEY_INVENTORY_QUESTS = Key.T;

	// Token: 0x040044BA RID: 17594
	public const Key DEFAULT_KEY_TAUNT = Key.V;

	// Token: 0x040044BB RID: 17595
	public const Key DEFAULT_KEY_UP = Key.UpArrow;

	// Token: 0x040044BC RID: 17596
	public const Key DEFAULT_KEY_DOWN = Key.DownArrow;

	// Token: 0x040044BD RID: 17597
	public const Key DEFAULT_KEY_LEFT = Key.LeftArrow;

	// Token: 0x040044BE RID: 17598
	public const Key DEFAULT_KEY_RIGHT = Key.RightArrow;

	// Token: 0x040044BF RID: 17599
	public const InputControlType BUTTON_DEFAULT_JUMP = InputControlType.Action1;

	// Token: 0x040044C0 RID: 17600
	public const InputControlType BUTTON_DEFAULT_ATTACK = InputControlType.Action3;

	// Token: 0x040044C1 RID: 17601
	public const InputControlType BUTTON_DEFAULT_CAST = InputControlType.Action2;

	// Token: 0x040044C2 RID: 17602
	public const InputControlType BUTTON_DEFAULT_DASH = InputControlType.RightTrigger;

	// Token: 0x040044C3 RID: 17603
	public const InputControlType BUTTON_DEFAULT_SUPERDASH = InputControlType.LeftTrigger;

	// Token: 0x040044C4 RID: 17604
	public const InputControlType BUTTON_DEFAULT_DREAMNAIL = InputControlType.Action4;

	// Token: 0x040044C5 RID: 17605
	public const InputControlType BUTTON_DEFAULT_QUICKMAP = InputControlType.LeftBumper;

	// Token: 0x040044C6 RID: 17606
	public const InputControlType BUTTON_DEFAULT_QUICKCAST = InputControlType.RightBumper;

	// Token: 0x040044C7 RID: 17607
	public const InputControlType BUTTON_DEFAULT_TAUNT = InputControlType.RightStickButton;

	// Token: 0x040044C8 RID: 17608
	public const InputControlType BUTTON_DEFAULT_INVENTORY = InputControlType.Back;

	// Token: 0x040044C9 RID: 17609
	public const InputControlType BUTTON_DEFAULT_PS4_INVENTORY = InputControlType.TouchPadButton;

	// Token: 0x040044CA RID: 17610
	public const InputControlType BUTTON_DEFAULT_PS4_PAUSE = InputControlType.Options;

	// Token: 0x040044CB RID: 17611
	public const InputControlType BUTTON_DEFAULT_PS5_INVENTORY = InputControlType.TouchPadButton;

	// Token: 0x040044CC RID: 17612
	public const InputControlType BUTTON_DEFAULT_PS5_PAUSE = InputControlType.Options;

	// Token: 0x040044CD RID: 17613
	public const InputControlType BUTTON_DEFAULT_XBONE_INVENTORY = InputControlType.View;

	// Token: 0x040044CE RID: 17614
	public const InputControlType BUTTON_DEFAULT_XBONE_PAUSE = InputControlType.Menu;

	// Token: 0x040044CF RID: 17615
	public const InputControlType BUTTON_DEFAULT_PS3_WIN_INVENTORY = InputControlType.Select;

	// Token: 0x040044D0 RID: 17616
	public const InputControlType BUTTON_DEFAULT_SWITCH_INVENTORY = InputControlType.Minus;

	// Token: 0x040044D1 RID: 17617
	public const InputControlType BUTTON_DEFAULT_SWITCH_PAUSE = InputControlType.Plus;

	// Token: 0x040044D2 RID: 17618
	public const InputControlType BUTTON_DEFAULT_UNKNOWN_INVENTORY = InputControlType.Select;

	// Token: 0x040044D3 RID: 17619
	public const string GSKEY_GAME_LANGUAGE = "GameLang";

	// Token: 0x040044D4 RID: 17620
	public const string GSKEY_GAME_BACKERS = "GameBackers";

	// Token: 0x040044D5 RID: 17621
	public const string GSKEY_GAME_POPUPS = "GameNativePopups";

	// Token: 0x040044D6 RID: 17622
	public const string GSKEY_RUMBLE_SETTING = "RumbleSetting";

	// Token: 0x040044D7 RID: 17623
	public const string GSKEY_CAMSHAKE_SETTING = "CameraShakeSetting";

	// Token: 0x040044D8 RID: 17624
	public const string GSKEY_HUDSCALE_SETTING = "HudScaleSetting";

	// Token: 0x040044D9 RID: 17625
	public const string GSKEY_VIDEO_RESX = "VidResX";

	// Token: 0x040044DA RID: 17626
	public const string GSKEY_VIDEO_RESY = "VidResY";

	// Token: 0x040044DB RID: 17627
	public const string GSKEY_VIDEO_REFRESH = "VidResRefresh";

	// Token: 0x040044DC RID: 17628
	public const string GSKEY_VIDEO_FULLSCREEN = "VidFullscreen";

	// Token: 0x040044DD RID: 17629
	public const string GSKEY_VIDEO_VSYNC = "VidVSync";

	// Token: 0x040044DE RID: 17630
	public const string GSKEY_VIDEO_DISPLAY = "VidDisplay";

	// Token: 0x040044DF RID: 17631
	public const string GSKEY_VIDEO_FRAME_CAP = "VidTFR";

	// Token: 0x040044E0 RID: 17632
	public const string GSKEY_VIDEO_PARTICLES = "VidParticles";

	// Token: 0x040044E1 RID: 17633
	public const string GSKEY_VIDEO_SHADER_QUALITY = "ShaderQuality";

	// Token: 0x040044E2 RID: 17634
	public const string GSKEY_OS_VALUE = "VidOSValue";

	// Token: 0x040044E3 RID: 17635
	public const string GSKEY_OS_SET = "VidOSSet";

	// Token: 0x040044E4 RID: 17636
	public const string GSKEY_BRIGHT_VALUE = "VidBrightValue";

	// Token: 0x040044E5 RID: 17637
	public const string GSKEY_BRIGHT_SET = "VidBrightSet";

	// Token: 0x040044E6 RID: 17638
	public const string GSKEY_AUDIO_MASTER = "MasterVolume";

	// Token: 0x040044E7 RID: 17639
	public const string GSKEY_AUDIO_MUSIC = "MusicVolume";

	// Token: 0x040044E8 RID: 17640
	public const string GSKEY_AUDIO_SOUND = "SoundVolume";

	// Token: 0x040044E9 RID: 17641
	public const string GSKEY_AUDIO_PLAYERVOICE = "PlayerVoiceEnabled";

	// Token: 0x040044EA RID: 17642
	public const string GSKEY_KEY_JUMP = "KeyJump";

	// Token: 0x040044EB RID: 17643
	public const string GSKEY_KEY_ATTACK = "KeyAttack";

	// Token: 0x040044EC RID: 17644
	public const string GSKEY_KEY_DASH = "KeyDash";

	// Token: 0x040044ED RID: 17645
	public const string GSKEY_KEY_CAST = "KeyCast";

	// Token: 0x040044EE RID: 17646
	public const string GSKEY_KEY_SUPERDASH = "KeySupDash";

	// Token: 0x040044EF RID: 17647
	public const string GSKEY_KEY_DREAMNAIL = "KeyDreamnail";

	// Token: 0x040044F0 RID: 17648
	public const string GSKEY_KEY_QUICKMAP = "KeyQuickMap";

	// Token: 0x040044F1 RID: 17649
	public const string GSKEY_KEY_QUICKCAST = "KeyQuickCast";

	// Token: 0x040044F2 RID: 17650
	public const string GSKEY_KEY_TAUNT = "KeyTaunt";

	// Token: 0x040044F3 RID: 17651
	public const string GSKEY_KEY_INVENTORY = "KeyInventory";

	// Token: 0x040044F4 RID: 17652
	public const string GSKEY_KEY_INVENTORY_MAP = "KeyInventoryMap";

	// Token: 0x040044F5 RID: 17653
	public const string GSKEY_KEY_INVENTORY_JOURNAL = "KeyInventoryJournal";

	// Token: 0x040044F6 RID: 17654
	public const string GSKEY_KEY_INVENTORY_TOOLS = "KeyInventoryTools";

	// Token: 0x040044F7 RID: 17655
	public const string GSKEY_KEY_INVENTORY_QUESTS = "KeyInventoryQuests";

	// Token: 0x040044F8 RID: 17656
	public const string GSKEY_KEY_UP = "KeyUp";

	// Token: 0x040044F9 RID: 17657
	public const string GSKEY_KEY_DOWN = "KeyDown";

	// Token: 0x040044FA RID: 17658
	public const string GSKEY_KEY_LEFT = "KeyLeft";

	// Token: 0x040044FB RID: 17659
	public const string GSKEY_KEY_RIGHT = "KeyRight";

	// Token: 0x040044FC RID: 17660
	public const string GSKEY_CONTROLLER_PREFIX = "Controller";

	// Token: 0x040044FD RID: 17661
	public const string GSKEY_LANG_SET = "GameLangSet";

	// Token: 0x040044FE RID: 17662
	public const string COMM_ARG_RESETALL = "-resetall";

	// Token: 0x040044FF RID: 17663
	public const string COMM_ARG_RESETRES = "-resetres";

	// Token: 0x04004500 RID: 17664
	public const string COMM_ARG_ALLOWLANG = "-forcelang";

	// Token: 0x04004501 RID: 17665
	public const string COMM_ARG_DEBUGKEYS = "-allowdebug";

	// Token: 0x04004502 RID: 17666
	public const string COMM_ARG_NATIVEINPUT = "-nativeinput";

	// Token: 0x04004503 RID: 17667
	private static readonly FieldCache fieldCache = new FieldCache(typeof(Constants));
}
