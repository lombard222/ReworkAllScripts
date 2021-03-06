using System;
using System.Linq;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;

namespace Pugna
{
	internal class Program
	{

		private static bool activated;
		private static Item orchid, sheep, vail, soulring, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost;
		private static Ability Q, W, E, R;
		private static bool toggle;
		private static bool blinkToggle = true;
		private static bool useUltimate = true;
		private static Font txt;
		private static Font noti;
		private static Line lines;
		private static bool autoUlt = true;
		private static Key keyCombo = Key.D;
		private static Key toggleKey = Key.M;
		private static Key blinkToggleKey = Key.P;
		private static Key UseUltimate = Key.H;


		static void Main(string[] args)
		{

			Game.OnUpdate += Game_OnUpdate;
			Game.OnWndProc += Game_OnWndProc;
			Console.WriteLine("> Pugna# loaded!");

			txt = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Segoe UI",
				   Height = 17,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });

			noti = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Segoe UI",
				   Height = 30,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });

			lines = new Line(Drawing.Direct3DDevice9);

			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}

		public static void Game_OnUpdate(EventArgs args)
		{
			var me = ObjectMgr.LocalHero;

			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Pugna || me == null)
			{
				return;
			}

			var target = me.ClosestToMouseTarget(2000);
			if (target == null)
			{
				return;
			}

			//spell
			Q = me.Spellbook.SpellQ;

			W = me.Spellbook.SpellW;

			E = me.Spellbook.SpellE;

			R = me.Spellbook.SpellR;

			// Item
			ethereal = me.FindItem("item_ethereal_blade");

			sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");

			vail = me.FindItem("item_veil_of_discord");

			cheese = me.FindItem("item_cheese");

			ghost = me.FindItem("item_ghost");

			orchid = me.FindItem("item_orchid");

			atos = me.FindItem("item_rod_of_atos");

			soulring = me.FindItem("item_soul_ring");

			arcane = me.FindItem("item_arcane_boots");

			blink = me.FindItem("item_blink");

			shiva = me.FindItem("item_shivas_guard");

			dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));




			
			var ModifRod = target.Modifiers.Any(y => y.Name == "modifier_rod_of_atos_debuff");
			var ModifEther = target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
			var ModifVail = target.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff");
			var stoneModif = target.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");


			if (activated && me.IsAlive && target.IsAlive && Utils.SleepCheck("activated"))
			{
				var noBlade = target.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect");
				if (R.IsInAbilityPhase || me.Modifiers.Any(y => y.Name == "modifier_pugna_life_drain") )
					return;
				if (target.IsVisible && me.Distance2D(target) <= 2300 && !noBlade)
				{
					if ((!me.IsChanneling() && !me.AghanimState()) || me.AghanimState())
					{
						if (
										 blink != null
										 && W.CanBeCasted()
										 && me.CanCast()
										 && blinkToggle
										 && blink.CanBeCasted()
										 && me.Distance2D(target) > 600
										 && me.Distance2D(target) < 1150
										 && !stoneModif
										 && Utils.SleepCheck("blink")
							   )
						{
							blink.UseAbility(target.Position);
							Utils.Sleep(250, "blink");
						}
						if (
											 W != null
											 && W.CanBeCasted()
											 && (target.IsLinkensProtected()
											 || !target.IsLinkensProtected())
											 && me.CanCast()
											 && me.Distance2D(target) < 1400
											 && !stoneModif
											 && Utils.SleepCheck("W")
							   )
						{
							W.UseAbility(target);
							Utils.Sleep(200, "W");
						}
						if ( // atos Blade
									  atos != null
									 && atos.CanBeCasted()
									 && me.CanCast()
									 && !target.IsLinkensProtected()
									 && !target.IsMagicImmune()
									 && Utils.SleepCheck("atos")
									 && me.Distance2D(target) <= 2000
									  )
						{
							atos.UseAbility(target);
							Utils.Sleep(250, "atos");
						} // atos Item end
						if (!W.CanBeCasted() || W == null)
						{

							if ( // atos Blade
										   atos != null
										  && atos.CanBeCasted()
										  && me.CanCast()
										  && !target.IsLinkensProtected()
										  && !target.IsMagicImmune()
										  && Utils.SleepCheck("atos")
										  && me.Distance2D(target) <= 2000
										   )
							{
								atos.UseAbility(target);
								Utils.Sleep(250, "atos");
							} // atos Item end

							if (
												Q != null
											   && Q.CanBeCasted()
											   && me.CanCast()
											   && me.Distance2D(target) < 1400
											   && !stoneModif
											   && Utils.SleepCheck("Q")
											   )
							{
								Q.UseAbility(target.Position);
								Utils.Sleep(200, "Q");
							}
							if ( // orchid
									  orchid != null
									  && orchid.CanBeCasted()
									  && me.CanCast()
									  && !target.IsLinkensProtected()
									  && !target.IsMagicImmune()
									  && Utils.SleepCheck("orchid")
									  && me.Distance2D(target) <= 1400
									  && !stoneModif
								)
							{
								orchid.UseAbility(target);
								Utils.Sleep(250 + Game.Ping, "orchid");
							} // orchid Item end
							if (!orchid.CanBeCasted() || orchid == null)
							{
								if ( // vail
									   vail != null
									  && vail.CanBeCasted()
									  && me.CanCast()
									  && !ModifVail
									  && !target.IsMagicImmune()
									  && Utils.SleepCheck("vail")
									  && me.Distance2D(target) <= 1500
									  )
								{
									vail.UseAbility(target.Position);
									Utils.Sleep(250, "vail");
								} // orchid Item end
								if (!vail.CanBeCasted() || vail == null)
								{
									
									if (// ethereal
										   ethereal != null
										   && ethereal.CanBeCasted()
										   && me.CanCast()

										   && !target.IsLinkensProtected()
										   && !target.IsMagicImmune()
										   && !stoneModif
										   && Utils.SleepCheck("ethereal")
										  )
									{
										ethereal.UseAbility(target);
										Utils.Sleep(200, "ethereal");
									} // ethereal Item end
									if (!ethereal.CanBeCasted() || ethereal == null)
									{


										

										if (// SoulRing Item 
											soulring != null
											&& soulring.CanBeCasted()
											&& me.CanCast()
											&& me.Health / me.MaximumHealth <= 0.5
											&& me.Mana <= R.ManaCost
											)
										{
											soulring.UseAbility();
										} // SoulRing Item end

										if (// Arcane Boots Item
											arcane != null
											&& arcane.CanBeCasted()
											&& me.CanCast()
											&& me.Mana <= R.ManaCost
											)
										{
											arcane.UseAbility();
										} // Arcane Boots Item end

										if (//Ghost
											ghost != null
											&& ghost.CanBeCasted()
											&& me.CanCast()
											&& ((me.Position.Distance2D(target) < 300
											&& me.Health <= (me.MaximumHealth * 0.7))
											|| me.Health <= (me.MaximumHealth * 0.3))
											&& Utils.SleepCheck("Ghost"))
										{
											ghost.UseAbility();
											Utils.Sleep(250, "Ghost");
										}


										if (// Shiva Item
											shiva != null
											&& shiva.CanBeCasted()
											&& me.CanCast()
											&& !target.IsMagicImmune()
											&& Utils.SleepCheck("shiva")
											&& me.Distance2D(target) <= 600
											)

										{
											shiva.UseAbility();
											Utils.Sleep(250 + Game.Ping, "shiva");
										} // Shiva Item end





										if ( // sheep
											sheep != null
											&& sheep.CanBeCasted()
											&& me.CanCast()
											&& !target.IsLinkensProtected()
											&& !target.IsMagicImmune()
											&& Utils.SleepCheck("sheep")
											&& me.Distance2D(target) <= 1400
											&& !stoneModif
											)
										{
											sheep.UseAbility(target);
											Utils.Sleep(250 + Game.Ping, "sheep");
										} // sheep Item end

										if (// Dagon
											me.CanCast()
											&& dagon != null
											&& (ethereal == null
											|| (ModifEther
											|| ethereal.Cooldown < 17))
											&& !target.IsLinkensProtected()
											&& dagon.CanBeCasted()
											&& !target.IsMagicImmune()
											&& !stoneModif
											&& Utils.SleepCheck("dagon")
										   )
										{
											dagon.UseAbility(target);
											Utils.Sleep(200, "dagon");
										} // Dagon Item end

										if (
											 // cheese
											 cheese != null
											 && cheese.CanBeCasted()
											 && Utils.SleepCheck("cheese")
											 && me.Health <= (me.MaximumHealth * 0.3)
											 && me.Distance2D(target) <= 700)
										{
											cheese.UseAbility();
											Utils.Sleep(200 + Game.Ping, "cheese");
										} // cheese Item end

									}
								}
							}
						}
						if (me.Modifiers.All(y => y.Name == "modifier_pugna_life_drain"))
							return;
						//, , , , arcane, blink, , , atos, , , ghost;
						if (
									   (R != null
									   && R.CanBeCasted()
									   && !me.IsChanneling()
									   && !me.Modifiers.All(y => y.Name == "modifier_pugna_life_drain")
									   && useUltimate
									   && (!Q.CanBeCasted() || Q == null)
									   && (!W.CanBeCasted() || W == null)
									   && (!atos.CanBeCasted() || atos == null)
									   && (!orchid.CanBeCasted() || orchid == null)
									   && (!sheep.CanBeCasted() || sheep == null)
									   && (!dagon.CanBeCasted() || dagon == null)
									   && (!ethereal.CanBeCasted() || ethereal == null)
									   && (!cheese.CanBeCasted() || cheese == null)
									   && me.Position.Distance2D(target) < 1200
									   && !stoneModif)

									   && Utils.SleepCheck("R"))
						{
							R.UseAbility(target);
							Utils.Sleep(330, "R");
						}
						return;
					}
				}
				Utils.Sleep(200, "activated");
			}
		}
		static void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			var player = ObjectMgr.LocalPlayer;
			var me = ObjectMgr.LocalHero;
			if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Pugna)
				return;

			if (activated)
			{
				DrawBox(2, 510, 130, 20, 1, new ColorBGRA(0, 0, 100, 100));
				DrawFilledBox(2, 510, 130, 20, new ColorBGRA(0, 0, 0, 100));
				DrawShadowText("Pugna#: Comboing!", 4, 510, Color.DeepPink, txt);
			}
			if (toggle && !activated)
			{
				DrawBox(2, 530, 410, 54, 1, new ColorBGRA(0, 0, 100, 100));
				DrawFilledBox(2, 530, 410, 54, new ColorBGRA(0, 0, 0, 100));
				DrawShadowText("Pugna#: Enabled\nBlink on/off(P): " + blinkToggle + " | UseUlt on/off(H): " + useUltimate + " | [" + keyCombo + "] for combo \n[" + toggleKey + "] For toggle combo | [" + blinkToggleKey +
					"] For toggle blink | [" + UseUltimate + "] For toggle UseUlt ", 4, 530, Color.OrangeRed, txt);
			}
			if (!toggle)
			{
				DrawBox(2, 530, 125, 20, 1, new ColorBGRA(0, 0, 100, 100));
				DrawFilledBox(2, 530, 125, 20, new ColorBGRA(0, 0, 0, 100));
				DrawShadowText("Open MENU |-->[" + toggleKey + "]", 4, 530, Color.DeepPink, txt);
			}
		}

		private static void Game_OnWndProc(WndEventArgs args)
		{
			if (!Game.IsChatOpen)
			{
				if (Game.IsKeyDown(keyCombo))
					activated = true;
				else
					activated = false;

				if (Game.IsKeyDown(toggleKey) && Utils.SleepCheck("toggle"))
				{
					toggle = !toggle;
					Utils.Sleep(150, "toggle");
				}

				if (Game.IsKeyDown(UseUltimate) && Utils.SleepCheck("useUltimate"))
				{
					useUltimate = !useUltimate;
					Utils.Sleep(150, "useUltimate");
				}

				if (Game.IsKeyDown(blinkToggleKey) && Utils.SleepCheck("toggleBlink"))
				{
					blinkToggle = !blinkToggle;
					Utils.Sleep(150, "toggleBlink");
				}
			}
		}

		static void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			txt.Dispose();
			noti.Dispose();
			lines.Dispose();
		}



		static void Drawing_OnPostReset(EventArgs args)
		{
			txt.OnResetDevice();
			noti.OnResetDevice();
			lines.OnResetDevice();
		}

		static void Drawing_OnPreReset(EventArgs args)
		{
			txt.OnLostDevice();
			noti.OnLostDevice();
			lines.OnLostDevice();
		}

		public static void DrawFilledBox(float x, float y, float w, float h, Color color)
		{
			var vLine = new Vector2[2];

			lines.GLLines = true;
			lines.Antialias = false;
			lines.Width = w;

			vLine[0].X = x + w / 2;
			vLine[0].Y = y;
			vLine[1].X = x + w / 2;
			vLine[1].Y = y + h;

			lines.Begin();
			lines.Draw(vLine, color);
			lines.End();
		}

		public static void DrawBox(float x, float y, float w, float h, float px, Color color)
		{
			DrawFilledBox(x, y + h, w, px, color);
			DrawFilledBox(x - px, y, px, h, color);
			DrawFilledBox(x, y - px, w, px, color);
			DrawFilledBox(x + w, y, px, h, color);
		}

		public static void DrawShadowText(string stext, int x, int y, Color color, Font f)
		{
			f.DrawText(null, stext, x + 1, y + 1, Color.Black);
			f.DrawText(null, stext, x, y, color);
		}
	}
}


