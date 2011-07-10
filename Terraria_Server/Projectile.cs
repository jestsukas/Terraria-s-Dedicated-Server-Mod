
using System;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
using Terraria_Server.Misc;
using Terraria_Server.Definitions;
using Terraria_Server.Collections;

namespace Terraria_Server
{
    /// <summary>
    /// Projectile includes things like bullets, arrows, knives, explosives, boomerangs, and possibly ball/chain, orbs, and flamelash/spells.
    /// </summary>
    public class Projectile : IRegisterableEntity
    {
        /// <summary>
        /// Whether the projectile is currently wet
        /// </summary>
        public bool wet;
        /// <summary>
        /// 
        /// </summary>
        public byte wetCount;
        /// <summary>
        /// Whether the projectile is currently immersed in lava
        /// </summary>
        public bool lavaWet;
        /// <summary>
        /// Projectile index
        /// </summary>
        public int whoAmI;
        /// <summary>
        /// 
        /// </summary>
        public const int MAX_AI = 2;
        /// <summary>
        /// Projectile's current location
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// Projectile's current 2-direction speed
        /// </summary>
        public Vector2 Velocity;
        /// <summary>
        /// Projectile's width of area effect
        /// </summary>
        public int width;
        /// <summary>
        /// Projectile's height of area effect
        /// </summary>
        public int height;
        /// <summary>
        /// Scaled size of projectile
        /// </summary>
        public float scale;
        /// <summary>
        /// Degrees of rotation for projectile sprite
        /// </summary>
        public float rotation;
        /// <summary>
        /// Projectile type
        /// </summary>
        public ProjectileType type { get; set; }

        public int Type
        {
            get
            {
                return (int)type;
            }
            set
            {
                type = (ProjectileType)value;
            }
        }

        /// <summary>
        /// Projectile's visibility, 255 == fully visible, 0 == invisible
        /// </summary>
        public int alpha;
        /// <summary>
        /// Index of owning player
        /// </summary>
        public int Owner = 255;
        /// <summary>
        /// Whether the projectile is currently alive in game
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Textual name of projectile type
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float[] ai = new float[Projectile.MAX_AI];
        /// <summary>
        /// Value of artificial intelligence style to use for motion
        /// </summary>
        public int aiStyle;
        /// <summary>
        /// Amount of time left in the projectile's life cycle?
        /// </summary>
        public int timeLeft;
        /// <summary>
        /// Amount of time to delay sound.  Unit unknown (likely milliseconds)
        /// </summary>
        public int soundDelay;
        /// <summary>
        /// Amount of damage the projectile can cause on a target entity
        /// </summary>
        public int damage;
        /// <summary>
        /// Integer representing x direction of travel.  Values unknown
        /// </summary>
        public int direction;
        /// <summary>
        ///  
        /// </summary>
        public bool hostile;
        /// <summary>
        /// Amount of knockback the projectile causes
        /// </summary>
        public float knockBack;
        /// <summary>
        /// 
        /// </summary>
        public bool friendly;
        /// <summary>
        /// How many entities the projectile can penetrate
        /// </summary>
        public int penetrate = 1;
        /// <summary>
        /// Projectile's index number in Main.projectile[]
        /// </summary>
        public int identity;
        /// <summary>
        /// Amount of light the projectile gives off
        /// </summary>
        public float light;
        /// <summary>
        /// 
        /// </summary>
        public bool netUpdate;
        /// <summary>
        /// Delay before striking an entity the projectile has already struck
        /// </summary>
        public int restrikeDelay;
        /// <summary>
        /// Whether the projectile has struck a solid tile
        /// </summary>
        public bool tileCollide;
        /// <summary>
        /// 
        /// </summary>
        public int maxUpdates;
        /// <summary>
        /// 
        /// </summary>
        public int numUpdates;
        /// <summary>
        /// Whether or not to ignore water conditional effects
        /// </summary>
        public bool ignoreWater;
        /// <summary>
        /// 
        /// </summary>
        public bool hide;
        /// <summary>
        /// 
        /// </summary>
        public bool ownerHitCheck;
        /// <summary>
        /// Any immune players
        /// </summary>
        public int[] playerImmune = new int[255];
        /// <summary>
        /// Miscellaneous text associated with the projectile.  Only used in hardcore deaths and sign edits?
        /// </summary>
        public String miscText = "";

        public Projectile()
        {
            for (int i = 0; i < Projectile.MAX_AI; i++)
            {
                this.ai[i] = 0f;
            }
            for (int j = 0; j < 255; j++)
            {
                this.playerImmune[j] = 0;
            }
            penetrate = 1;
            tileCollide = true;
            Position = default(Vector2);
            Velocity = default(Vector2);
            scale = 1f;
            Owner = 255;
            timeLeft = 3600;
            Name = "";
            miscText = "";
            width = (int)((float)this.width * this.scale);
            height = (int)((float)this.height * this.scale);
        }

        /// <summary>
        /// Creates a copy of the projectile's instance
        /// </summary>
        /// <returns>Copy of the projectile instance</returns>
        public object Clone()
        {
            return base.MemberwiseClone();
        }

        /// <summary>
        /// Creates a new projectile instance with the specified parameters
        /// </summary>
        /// <param name="X">Starting X coordinate</param>
        /// <param name="Y">Starting Y coordinate</param>
        /// <param name="SpeedX">Starting horizontal speed</param>
        /// <param name="SpeedY">Starting vertical speed</param>
        /// <param name="Type">Type of projectile to create</param>
        /// <param name="Damage">Amount of damage the projectile takes before self-destructing? (unknown)</param>
        /// <param name="KnockBack">Whether the projectile creates knockback</param>
        /// <param name="Owner">Index of owning player</param>
        /// <returns>New projectile's index</returns>
        public static int NewProjectile(float X, float Y, float SpeedX, float SpeedY, ProjectileType Type, int Damage, float KnockBack, int Owner = 255)
        {
            int num = 1000;
            for (int i = 0; i < 1000; i++)
            {
                if (!Main.projectile[i].Active)
                {
                    num = i;
                    break;
                }
            }
            if (num == 1000)
            {
                return num;
            }
            Main.projectile[num] = Registries.Projectile.Create((int)Type);
            Main.projectile[num].Position.X = X - (float)Main.projectile[num].width * 0.5f;
            Main.projectile[num].Position.Y = Y - (float)Main.projectile[num].height * 0.5f;
            Main.projectile[num].Owner = Owner;
            Main.projectile[num].Velocity.X = SpeedX;
            Main.projectile[num].Velocity.Y = SpeedY;
            Main.projectile[num].damage = Damage;
            Main.projectile[num].knockBack = KnockBack;
            Main.projectile[num].identity = num;
            Main.projectile[num].wet = Collision.WetCollision(Main.projectile[num].Position, Main.projectile[num].width, Main.projectile[num].height);
            if (Owner == Main.myPlayer)
            {
                NetMessage.SendData(27, -1, -1, "", num);
            }
            if (Owner == Main.myPlayer)
            {
                if (Type == ProjectileType.BOMB)
                {
                    Main.projectile[num].timeLeft = 180;
                }
                if (Type == ProjectileType.DYNAMITE)
                {
                    Main.projectile[num].timeLeft = 300;
                }
                if (Type == ProjectileType.GRENADE)
                {
                    Main.projectile[num].timeLeft = 180;
                }
                if (Type == ProjectileType.BOMB_STICKY)
                {
                    Main.projectile[num].timeLeft = 180;
                }
            }
            return num;
        }

        /// <summary>
        /// Runs damage calculation on hostile mobs and players
        /// </summary>
        public void Damage()
        {
            int playerIndex = Main.myPlayer;
            Player player = Main.players[playerIndex];

            Rectangle rectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.width, this.height);
            if (this.friendly && this.type != ProjectileType.ORB_OF_LIGHT)
            {
                if (this.Owner == playerIndex)
                {
                    if ((this.aiStyle == 16 || this.type == ProjectileType.ARROW_HELLFIRE) && this.timeLeft <= 1)
                    {
                        if (player.Active && !player.dead && !player.immune && (!this.ownerHitCheck || Collision.CanHit(Main.players[this.Owner].Position, Main.players[this.Owner].width, Main.players[this.Owner].height, player.Position, player.width, player.height)))
                        {
                            if (player.Intersects(rectangle))
                            {
                                if (player.Position.X + (float)(player.width / 2) < this.Position.X + (float)(this.width / 2))
                                {
                                    this.direction = -1;
                                }
                                else
                                {
                                    this.direction = 1;
                                }
                                player.Hurt(this.damage, this.direction, true, false, Player.getDeathMessage(this.Owner, -1, this.whoAmI, -1));
                                
                                NetMessage.SendData(26, -1, -1, Player.getDeathMessage(this.Owner, -1, this.whoAmI, -1), playerIndex, (float)this.direction, (float)this.damage, 1f);
                            }
                        }
                    }
                    int num = (int)(this.Position.X / 16f);
                    int num2 = (int)((this.Position.X + (float)this.width) / 16f) + 1;
                    int num3 = (int)(this.Position.Y / 16f);
                    int num4 = (int)((this.Position.Y + (float)this.height) / 16f) + 1;
                    if (num < 0)
                    {
                        num = 0;
                    }
                    if (num2 > Main.maxTilesX)
                    {
                        num2 = Main.maxTilesX;
                    }
                    if (num3 < 0)
                    {
                        num3 = 0;
                    }
                    if (num4 > Main.maxTilesY)
                    {
                        num4 = Main.maxTilesY;
                    }
                    for (int i = num; i < num2; i++)
                    {
                        for (int j = num3; j < num4; j++)
                        {
                            if (Main.tile[i, j] != null && Main.tileCut[(int)Main.tile[i, j].type] && Main.tile[i, j + 1] != null && Main.tile[i, j + 1].type != 78)
                            {
                                WorldGen.KillTile(i, j, false, false, false);
                                NetMessage.SendData(17, -1, -1, "", 0, (float)i, (float)j);
                            }
                        }
                    }
                    if (this.damage > 0)
                    {
                        NPC npc;
                        for (int i = 0; i < NPC.MAX_NPCS; i++)
                        {
                            npc = Main.npcs[i];
                            if (npc.Active && (!npc.friendly || (npc.Type == 22 && this.Owner < 255 && Main.players[this.Owner].killGuide)) && (this.Owner < 0 || npc.immune[this.Owner] == 0))
                            {
                                bool flag = false;
                                if (this.type == ProjectileType.POWDER_VILE && (npc.Type == 47 || npc.Type == 57))
                                {
                                    flag = true;
                                }
                                else
                                {
                                    if (this.type == ProjectileType.BALL_SAND_DROP && npc.Type == 69)
                                    {
                                        flag = true;
                                    }
                                }
                                if (!flag && (npc.noTileCollide || !this.ownerHitCheck || Collision.CanHit(Main.players[this.Owner].Position, Main.players[this.Owner].width, Main.players[this.Owner].height, npc.Position, npc.width, npc.height)))
                                {
                                    if(npc.Intersects(rectangle))
                                    {
                                        if (this.aiStyle == 3)
                                        {
                                            if (this.ai[0] == 0f)
                                            {
                                                this.Velocity.X = -this.Velocity.X;
                                                this.Velocity.Y = -this.Velocity.Y;
                                                this.netUpdate = true;
                                            }
                                            this.ai[0] = 1f;
                                        }
                                        else
                                        {
                                            if (this.aiStyle == 16)
                                            {
                                                if (this.timeLeft > 3)
                                                {
                                                    this.timeLeft = 3;
                                                }
                                                if (npc.Position.X + (float)(npc.width / 2) < this.Position.X + (float)(this.width / 2))
                                                {
                                                    this.direction = -1;
                                                }
                                                else
                                                {
                                                    this.direction = 1;
                                                }
                                            }
                                        }
                                        if (this.type == ProjectileType.ARROW_HELLFIRE && this.timeLeft > 1)
                                        {
                                            this.timeLeft = 1;
                                        }
                                        npc.StrikeNPC(this.damage, this.knockBack, this.direction);
                                        
                                        NetMessage.SendData(28, -1, -1, "", i, (float)this.damage, this.knockBack, (float)this.direction);

                                        if (this.penetrate != 1)
                                        {
                                            npc.immune[this.Owner] = 10;
                                        }
                                        if (this.penetrate > 0)
                                        {
                                            this.penetrate--;
                                            if (this.penetrate == 0)
                                            {
                                                break;
                                            }
                                        }
                                        if (this.aiStyle == 7)
                                        {
                                            this.ai[0] = 1f;
                                            this.damage = 0;
                                            this.netUpdate = true;
                                        }
                                        else
                                        {
                                            if (this.aiStyle == 13)
                                            {
                                                this.ai[0] = 1f;
                                                this.netUpdate = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (this.damage > 0 && player.hostile)
                    {
                        Player playerIt;
                        for (int i = 0; i < Main.MAX_PLAYERS; i++)
                        {
                            playerIt = Main.players[i];
                            if (i != this.Owner && playerIt.Active && !playerIt.dead && !playerIt.immune && playerIt.hostile && this.playerImmune[i] <= 0 && (player.team == 0 || player.team != playerIt.team) && (!this.ownerHitCheck || Collision.CanHit(Main.players[this.Owner].Position, Main.players[this.Owner].width, Main.players[this.Owner].height, playerIt.Position, playerIt.width, playerIt.height)))
                            {
                                if (playerIt.Intersects(rectangle))
                                {
                                    if (this.aiStyle == 3)
                                    {
                                        if (this.ai[0] == 0f)
                                        {
                                            this.Velocity.X = -this.Velocity.X;
                                            this.Velocity.Y = -this.Velocity.Y;
                                            this.netUpdate = true;
                                        }
                                        this.ai[0] = 1f;
                                    }
                                    else
                                    {
                                        if (this.aiStyle == 16)
                                        {
                                            if (this.timeLeft > 3)
                                            {
                                                this.timeLeft = 3;
                                            }
                                            if (playerIt.Position.X + (float)(playerIt.width / 2) < this.Position.X + (float)(this.width / 2))
                                            {
                                                this.direction = -1;
                                            }
                                            else
                                            {
                                                this.direction = 1;
                                            }
                                        }
                                    }
                                    if (this.type == ProjectileType.ARROW_HELLFIRE && this.timeLeft > 1)
                                    {
                                        this.timeLeft = 1;
                                    }
                                    playerIt.Hurt(this.damage, this.direction, true, false, Player.getDeathMessage(this.Owner, -1, this.whoAmI, -1));
                                                                        
                                    NetMessage.SendData(26, -1, -1, Player.getDeathMessage(this.Owner, -1, this.whoAmI, -1), i, (float)this.direction, (float)this.damage, 1f);

                                    this.playerImmune[i] = 40;
                                    if (this.penetrate > 0)
                                    {
                                        this.penetrate--;
                                        if (this.penetrate == 0)
                                        {
                                            break;
                                        }
                                    }
                                    if (this.aiStyle == 7)
                                    {
                                        this.ai[0] = 1f;
                                        this.damage = 0;
                                        this.netUpdate = true;
                                    }
                                    else
                                    {
                                        if (this.aiStyle == 13)
                                        {
                                            this.ai[0] = 1f;
                                            this.netUpdate = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (this.type == ProjectileType.POWDER_VILE)
                {
                    NPC npc;
                    for (int i = 0; i < NPC.MAX_NPCS; i++)
                    {
                        npc = Main.npcs[i];
                        if (npc.Active)
                        {
                            if (npc.Type == 46)
                            {
                                if (npc.Intersects(rectangle))
                                {
                                    NPC.Transform(i, 47);
                                }
                            }
                            else
                            {
                                if (npc.Type == 55)
                                {
                                    if (npc.Intersects(rectangle))
                                    {
                                        NPC.Transform(i, 57);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (this.hostile && Main.myPlayer < 255 && this.damage > 0)
            {
                if (player.Active && !player.dead && !player.immune)
                {
                    if (player.Intersects(rectangle))
                    {
                        int hitDirection = this.direction;
                        if (player.Position.X + (float)(player.width / 2) < this.Position.X + (float)(this.width / 2))
                        {
                            hitDirection = -1;
                        }
                        else
                        {
                            hitDirection = 1;
                        }
                        player.Hurt(this.damage * 2, hitDirection, false, false, " was slain...");
                        
                        NetMessage.SendData(26, -1, -1, "", playerIndex, (float)this.direction, (float)(this.damage * 2));
                    }
                }
            }
        }

        /// <summary>
        /// Updates the projectile's position, damage variables, etc.
        /// </summary>
        /// <param name="i">Projectile index</param>
        public void Update(int i)
        {
            if (this.Active)
            {
                Vector2 value = this.Velocity;
                if (this.Position.X <= Main.leftWorld || this.Position.X + (float)this.width >= Main.rightWorld || this.Position.Y <= Main.topWorld || this.Position.Y + (float)this.height >= Main.bottomWorld)
                {
                    this.Active = false;
                    return;
                }
                this.whoAmI = i;
                if (this.soundDelay > 0)
                {
                    this.soundDelay--;
                }
                this.netUpdate = false;
                for (int j = 0; j < 255; j++)
                {
                    if (this.playerImmune[j] > 0)
                    {
                        this.playerImmune[j]--;
                    }
                }
                this.AI();
                if (this.Owner < 255 && !Main.players[this.Owner].Active)
                {
                    this.Kill();
                }
                if (!this.ignoreWater)
                {
                    bool flag;
                    bool flag2;
                    try
                    {
                        flag = Collision.LavaCollision(this.Position, this.width, this.height);
                        flag2 = Collision.WetCollision(this.Position, this.width, this.height);
                        if (flag)
                        {
                            this.lavaWet = true;
                        }
                    }
                    catch
                    {
                        this.Active = false;
                        return;
                    }
                    if (flag2)
                    {
                        if (this.wetCount == 0)
                        {
                            this.wetCount = 10;
                            if (!this.wet)
                            {
                                if (!flag)
                                {
                                    for (int k = 0; k < 10; k++)
                                    {
                                        int num = Dust.NewDust(new Vector2(this.Position.X - 6f, this.Position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 33, 0f, 0f, 0, default(Color), 1f);
                                        Dust expr_1E9_cp_0 = Main.dust[num];
                                        expr_1E9_cp_0.velocity.Y = expr_1E9_cp_0.velocity.Y - 4f;
                                        Dust expr_207_cp_0 = Main.dust[num];
                                        expr_207_cp_0.velocity.X = expr_207_cp_0.velocity.X * 2.5f;
                                        Main.dust[num].scale = 1.3f;
                                        Main.dust[num].alpha = 100;
                                        Main.dust[num].noGravity = true;
                                    }
                                }
                                else
                                {
                                    for (int l = 0; l < 10; l++)
                                    {
                                        int num2 = Dust.NewDust(new Vector2(this.Position.X - 6f, this.Position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 35, 0f, 0f, 0, default(Color), 1f);
                                        Dust expr_2EF_cp_0 = Main.dust[num2];
                                        expr_2EF_cp_0.velocity.Y = expr_2EF_cp_0.velocity.Y - 1.5f;
                                        Dust expr_30D_cp_0 = Main.dust[num2];
                                        expr_30D_cp_0.velocity.X = expr_30D_cp_0.velocity.X * 2.5f;
                                        Main.dust[num2].scale = 1.3f;
                                        Main.dust[num2].alpha = 100;
                                        Main.dust[num2].noGravity = true;
                                    }
                                }
                            }
                            this.wet = true;
                        }
                    }
                    else
                    {
                        if (this.wet)
                        {
                            this.wet = false;
                            if (this.wetCount == 0)
                            {
                                this.wetCount = 10;
                                if (!this.lavaWet)
                                {
                                    for (int m = 0; m < 10; m++)
                                    {
                                        int num3 = Dust.NewDust(new Vector2(this.Position.X - 6f, this.Position.Y + (float)(this.height / 2)), this.width + 12, 24, 33, 0f, 0f, 0, default(Color), 1f);
                                        Dust expr_426_cp_0 = Main.dust[num3];
                                        expr_426_cp_0.velocity.Y = expr_426_cp_0.velocity.Y - 4f;
                                        Dust expr_444_cp_0 = Main.dust[num3];
                                        expr_444_cp_0.velocity.X = expr_444_cp_0.velocity.X * 2.5f;
                                        Main.dust[num3].scale = 1.3f;
                                        Main.dust[num3].alpha = 100;
                                        Main.dust[num3].noGravity = true;
                                    }
                                }
                                else
                                {
                                    for (int n = 0; n < 10; n++)
                                    {
                                        int num4 = Dust.NewDust(new Vector2(this.Position.X - 6f, this.Position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 35, 0f, 0f, 0, default(Color), 1f);
                                        Dust expr_52C_cp_0 = Main.dust[num4];
                                        expr_52C_cp_0.velocity.Y = expr_52C_cp_0.velocity.Y - 1.5f;
                                        Dust expr_54A_cp_0 = Main.dust[num4];
                                        expr_54A_cp_0.velocity.X = expr_54A_cp_0.velocity.X * 2.5f;
                                        Main.dust[num4].scale = 1.3f;
                                        Main.dust[num4].alpha = 100;
                                        Main.dust[num4].noGravity = true;
                                    }
                                }
                            }
                        }
                    }
                    if (!this.wet)
                    {
                        this.lavaWet = false;
                    }
                    if (this.wetCount > 0)
                    {
                        this.wetCount -= 1;
                    }
                }
                if (this.tileCollide)
                {
                    Vector2 value2 = this.Velocity;
                    bool flag3 = true;
                    if (this.type == ProjectileType.STARFURY || this.type == ProjectileType.FALLEN_STAR || this.type == ProjectileType.BALL_OF_FIRE || this.type == ProjectileType.HOOK || this.type == ProjectileType.BALL_SAND_DROP || this.type == ProjectileType.BALL_MUD || this.type == ProjectileType.BALL_ASH)
                    {
                        flag3 = false;
                    }
                    if (this.aiStyle == 10)
                    {
                        if (this.type == ProjectileType.BALL_SAND_GUN || (this.type == ProjectileType.BALL_SAND_DROP && this.ai[0] == 2f))
                        {
                            this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.width, this.height, flag3, flag3);
                        }
                        else
                        {
                            this.Velocity = Collision.AnyCollision(this.Position, this.Velocity, this.width, this.height);
                        }
                    }
                    else
                    {
                        if (this.aiStyle == 18)
                        {
                            int num5 = this.width - 36;
                            int num6 = this.height - 36;
                            Vector2 vector = new Vector2(this.Position.X + (float)(this.width / 2) - (float)(num5 / 2), this.Position.Y + (float)(this.height / 2) - (float)(num6 / 2));
                            this.Velocity = Collision.TileCollision(vector, this.Velocity, num5, num6, flag3, flag3);
                        }
                        else
                        {
                            if (this.wet)
                            {
                                Vector2 vector2 = this.Velocity;
                                this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.width, this.height, flag3, flag3);
                                value = this.Velocity * 0.5f;
                                if (this.Velocity.X != vector2.X)
                                {
                                    value.X = this.Velocity.X;
                                }
                                if (this.Velocity.Y != vector2.Y)
                                {
                                    value.Y = this.Velocity.Y;
                                }
                            }
                            else
                            {
                                this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.width, this.height, flag3, flag3);
                            }
                        }
                    }
                    if (value2 != this.Velocity)
                    {
                        if (this.type == ProjectileType.SHOT_METEOR)
                        {
                            if (this.penetrate > 1)
                            {
                                Collision.HitTiles(this.Position, this.Velocity, this.width, this.height);
                                this.penetrate--;
                                if (this.Velocity.X != value2.X)
                                {
                                    this.Velocity.X = -value2.X;
                                }
                                if (this.Velocity.Y != value2.Y)
                                {
                                    this.Velocity.Y = -value2.Y;
                                }
                            }
                            else
                            {
                                this.Kill();
                            }
                        }
                        else
                        {
                            if (this.aiStyle == 17)
                            {
                                if (this.Velocity.X != value2.X)
                                {
                                    this.Velocity.X = value2.X * -0.75f;
                                }
                                if (this.Velocity.Y != value2.Y && (double)value2.Y > 1.5)
                                {
                                    this.Velocity.Y = value2.Y * -0.7f;
                                }
                            }
                            else
                            {
                                if (this.aiStyle == 3 || this.aiStyle == 13 || this.aiStyle == 15)
                                {
                                    Collision.HitTiles(this.Position, this.Velocity, this.width, this.height);
                                    if (this.type == ProjectileType.CHAKRUM_THORN)
                                    {
                                        if (this.Velocity.X != value2.X)
                                        {
                                            this.Velocity.X = -value2.X;
                                        }
                                        if (this.Velocity.Y != value2.Y)
                                        {
                                            this.Velocity.Y = -value2.Y;
                                        }
                                    }
                                    else
                                    {
                                        this.ai[0] = 1f;
                                        if (this.aiStyle == 3)
                                        {
                                            this.Velocity.X = -value2.X;
                                            this.Velocity.Y = -value2.Y;
                                        }
                                    }
                                    this.netUpdate = true;
                                }
                                else
                                {
                                    if (this.aiStyle == 8)
                                    {
                                        this.ai[0] += 1f;
                                        if (this.ai[0] >= 5f)
                                        {
                                            this.Position += this.Velocity;
                                            this.Kill();
                                        }
                                        else
                                        {
                                            if (this.Velocity.Y != value2.Y)
                                            {
                                                this.Velocity.Y = -value2.Y;
                                            }
                                            if (this.Velocity.X != value2.X)
                                            {
                                                this.Velocity.X = -value2.X;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (this.aiStyle == 14)
                                        {
                                            if (this.type == ProjectileType.GLOWSTICK)
                                            {
                                                if (this.Velocity.X != value2.X)
                                                {
                                                    this.Velocity.X = value2.X * -0.2f;
                                                }
                                                if (this.Velocity.Y != value2.Y && (double)value2.Y > 1.5)
                                                {
                                                    this.Velocity.Y = value2.Y * -0.2f;
                                                }
                                            }
                                            else
                                            {
                                                if (this.Velocity.X != value2.X)
                                                {
                                                    this.Velocity.X = value2.X * -0.5f;
                                                }
                                                if (this.Velocity.Y != value2.Y && value2.Y > 1f)
                                                {
                                                    this.Velocity.Y = value2.Y * -0.5f;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (this.aiStyle == 16)
                                            {
                                                if (this.Velocity.X != value2.X)
                                                {
                                                    this.Velocity.X = value2.X * -0.4f;
                                                    if (this.type == ProjectileType.DYNAMITE)
                                                    {
                                                        this.Velocity.X = this.Velocity.X * 0.8f;
                                                    }
                                                }
                                                if (this.Velocity.Y != value2.Y && (double)value2.Y > 0.7)
                                                {
                                                    this.Velocity.Y = value2.Y * -0.4f;
                                                    if (this.type == ProjectileType.DYNAMITE)
                                                    {
                                                        this.Velocity.Y = this.Velocity.Y * 0.8f;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                this.Position += this.Velocity;
                                                this.Kill();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (this.type == ProjectileType.VILETHORN || this.type == ProjectileType.VILETHORN_B)
                {
                    goto IL_D48;
                }
                if (this.wet)
                {
                    this.Position += value;
                    goto IL_D48;
                }
                this.Position += this.Velocity;
            IL_D48:
                if ((this.aiStyle != 3 || this.ai[0] != 1f) && (this.aiStyle != 7 || this.ai[0] != 1f) && (this.aiStyle != 13 || this.ai[0] != 1f) && (this.aiStyle != 15 || this.ai[0] != 1f))
                {
                    if (this.Velocity.X < 0f)
                    {
                        this.direction = -1;
                    }
                    else
                    {
                        this.direction = 1;
                    }
                }
                if (!this.Active)
                {
                    return;
                }

                if (this.type == ProjectileType.ARROW_FIRE)
                {
                    Dust.NewDust(new Vector2(this.Position.X, this.Position.Y), this.width, this.height, 6, 0f, 0f, 100, default(Color), 1f);
                }
                else
                {
                    if (this.type == ProjectileType.ARROW_UNHOLY)
                    {
                        if (Main.rand.Next(5) == 0)
                        {
                            Dust.NewDust(new Vector2(this.Position.X, this.Position.Y), this.width, this.height, 14, 0f, 0f, 150, default(Color), 1.1f);
                        }
                    }
                    else
                    {
                        if (this.type == ProjectileType.ARROW_JESTER)
                        {
                            Dust.NewDust(this.Position, this.width, this.height, 15, this.Velocity.X * 0.5f, this.Velocity.Y * 0.5f, 150, default(Color), 1.2f);
                        }
                    }
                }
                this.Damage();
                this.timeLeft--;
                if (this.timeLeft <= 0)
                {
                    this.Kill();
                }
                if (this.penetrate == 0)
                {
                    this.Kill();
                }
                if (this.Active && this.netUpdate && this.Owner == Main.myPlayer)
                {
                    NetMessage.SendData(27, -1, -1, "", i);
                }
                if (this.Active && this.maxUpdates > 0)
                {
                    this.numUpdates--;
                    if (this.numUpdates >= 0)
                    {
                        this.Update(i);
                    }
                    else
                    {
                        this.numUpdates = this.maxUpdates;
                    }
                }
                this.netUpdate = false;
            }
        }

        /// <summary>
        /// Moves the projectile according to the projectile's motion parameters, or AI
        /// </summary>
        public void AI()
        {
            if (this.aiStyle == 1)
            {
                if (this.type == ProjectileType.ARROW_HELLFIRE)
                {
                    Vector2 arg_5D_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_5D_1 = this.width;
                    int arg_5D_2 = this.height;
                    int arg_5D_3 = 31;
                    float arg_5D_4 = 0f;
                    float arg_5D_5 = 0f;
                    int arg_5D_6 = 100;
                    Color newColor = default(Color);
                    int num = Dust.NewDust(arg_5D_0, arg_5D_1, arg_5D_2, arg_5D_3, arg_5D_4, arg_5D_5, arg_5D_6, newColor, 1.6f);
                    Main.dust[num].noGravity = true;
                    Vector2 arg_B3_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_B3_1 = this.width;
                    int arg_B3_2 = this.height;
                    int arg_B3_3 = 6;
                    float arg_B3_4 = 0f;
                    float arg_B3_5 = 0f;
                    int arg_B3_6 = 100;
                    newColor = default(Color);
                    num = Dust.NewDust(arg_B3_0, arg_B3_1, arg_B3_2, arg_B3_3, arg_B3_4, arg_B3_5, arg_B3_6, newColor, 2f);
                    Main.dust[num].noGravity = true;
                }
                if (this.type == ProjectileType.LASER_GREEN || this.type == ProjectileType.BALL_MUSKET || this.type == ProjectileType.SHOT_METEOR)
                {
                    if (this.alpha > 0)
                    {
                        this.alpha -= 15;
                    }
                    if (this.alpha < 0)
                    {
                        this.alpha = 0;
                    }
                }
                if (this.type != ProjectileType.ARROW_JESTER && this.type != ProjectileType.BALL_MUSKET && this.type != ProjectileType.LASER_GREEN && this.type != ProjectileType.SHOT_METEOR && this.type != ProjectileType.FEATHER_HARPY)
                {
                    this.ai[0] += 1f;
                }
                if (this.ai[0] >= 15f)
                {
                    this.ai[0] = 15f;
                    this.Velocity.Y = this.Velocity.Y + 0.1f;
                }
                this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 1.57f;
                if (this.Velocity.Y > 16f)
                {
                    this.Velocity.Y = 16f;
                    return;
                }
            }
            else
            {
                if (this.aiStyle == 2)
                {
                    this.rotation += (Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y)) * 0.03f * (float)this.direction;
                    this.ai[0] += 1f;
                    if (this.ai[0] >= 20f)
                    {
                        this.Velocity.Y = this.Velocity.Y + 0.4f;
                        this.Velocity.X = this.Velocity.X * 0.97f;
                    }
                    else
                    {
                        if (this.type == ProjectileType.KNIFE_THROWING || this.type == ProjectileType.KNIFE_POISONED)
                        {
                            this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 1.57f;
                        }
                    }
                    if (this.Velocity.Y > 16f)
                    {
                        this.Velocity.Y = 16f;
                    }
                    if (this.type == ProjectileType.KNIFE_POISONED && Main.rand.Next(20) == 0)
                    {
                        Vector2 arg_35A_0 = new Vector2(this.Position.X, this.Position.Y);
                        int arg_35A_1 = this.width;
                        int arg_35A_2 = this.height;
                        int arg_35A_3 = 40;
                        float arg_35A_4 = this.Velocity.X * 0.1f;
                        float arg_35A_5 = this.Velocity.Y * 0.1f;
                        int arg_35A_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_35A_0, arg_35A_1, arg_35A_2, arg_35A_3, arg_35A_4, arg_35A_5, arg_35A_6, newColor, 0.75f);
                        return;
                    }
                }
                else
                {
                    if (this.aiStyle == 3)
                    {
                        if (this.soundDelay == 0)
                        {
                            this.soundDelay = 8;
                        }
                        if (this.type == ProjectileType.FLAMARANG)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 arg_40A_0 = new Vector2(this.Position.X, this.Position.Y);
                                int arg_40A_1 = this.width;
                                int arg_40A_2 = this.height;
                                int arg_40A_3 = 6;
                                float arg_40A_4 = this.Velocity.X * 0.2f;
                                float arg_40A_5 = this.Velocity.Y * 0.2f;
                                int arg_40A_6 = 100;
                                Color newColor = default(Color);
                                int num2 = Dust.NewDust(arg_40A_0, arg_40A_1, arg_40A_2, arg_40A_3, arg_40A_4, arg_40A_5, arg_40A_6, newColor, 2f);
                                Main.dust[num2].noGravity = true;
                                Dust expr_429_cp_0 = Main.dust[num2];
                                expr_429_cp_0.velocity.X = expr_429_cp_0.velocity.X * 0.3f;
                                Dust expr_446_cp_0 = Main.dust[num2];
                                expr_446_cp_0.velocity.Y = expr_446_cp_0.velocity.Y * 0.3f;
                            }
                        }
                        else
                        {
                            if (this.type == ProjectileType.CHAKRUM_THORN)
                            {
                                if (Main.rand.Next(1) == 0)
                                {
                                    Vector2 arg_4C7_0 = this.Position;
                                    int arg_4C7_1 = this.width;
                                    int arg_4C7_2 = this.height;
                                    int arg_4C7_3 = 40;
                                    float arg_4C7_4 = this.Velocity.X * 0.25f;
                                    float arg_4C7_5 = this.Velocity.Y * 0.25f;
                                    int arg_4C7_6 = 0;
                                    Color newColor = default(Color);
                                    int num3 = Dust.NewDust(arg_4C7_0, arg_4C7_1, arg_4C7_2, arg_4C7_3, arg_4C7_4, arg_4C7_5, arg_4C7_6, newColor, 1.4f);
                                    Main.dust[num3].noGravity = true;
                                }
                            }
                            else
                            {
                                if (this.type == ProjectileType.BOOMERANG_ENCHANTED && Main.rand.Next(5) == 0)
                                {
                                    Vector2 arg_53C_0 = this.Position;
                                    int arg_53C_1 = this.width;
                                    int arg_53C_2 = this.height;
                                    int arg_53C_3 = 15;
                                    float arg_53C_4 = this.Velocity.X * 0.5f;
                                    float arg_53C_5 = this.Velocity.Y * 0.5f;
                                    int arg_53C_6 = 150;
                                    Color newColor = default(Color);
                                    Dust.NewDust(arg_53C_0, arg_53C_1, arg_53C_2, arg_53C_3, arg_53C_4, arg_53C_5, arg_53C_6, newColor, 0.9f);
                                }
                            }
                        }
                        if (this.ai[0] == 0f)
                        {
                            this.ai[1] += 1f;
                            if (this.ai[1] >= 30f)
                            {
                                this.ai[0] = 1f;
                                this.ai[1] = 0f;
                                this.netUpdate = true;
                            }
                        }
                        else
                        {
                            this.tileCollide = false;
                            float num4 = 9f;
                            float num5 = 0.4f;
                            if (this.type == ProjectileType.FLAMARANG)
                            {
                                num4 = 13f;
                                num5 = 0.6f;
                            }
                            else
                            {
                                if (this.type == ProjectileType.CHAKRUM_THORN)
                                {
                                    num4 = 15f;
                                    num5 = 0.8f;
                                }
                            }
                            Vector2 vector = new Vector2(this.Position.X + (float)this.width * 0.5f, this.Position.Y + (float)this.height * 0.5f);
                            float num6 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].width / 2) - vector.X;
                            float num7 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].height / 2) - vector.Y;
                            float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                            num8 = num4 / num8;
                            num6 *= num8;
                            num7 *= num8;
                            if (this.Velocity.X < num6)
                            {
                                this.Velocity.X = this.Velocity.X + num5;
                                if (this.Velocity.X < 0f && num6 > 0f)
                                {
                                    this.Velocity.X = this.Velocity.X + num5;
                                }
                            }
                            else
                            {
                                if (this.Velocity.X > num6)
                                {
                                    this.Velocity.X = this.Velocity.X - num5;
                                    if (this.Velocity.X > 0f && num6 < 0f)
                                    {
                                        this.Velocity.X = this.Velocity.X - num5;
                                    }
                                }
                            }
                            if (this.Velocity.Y < num7)
                            {
                                this.Velocity.Y = this.Velocity.Y + num5;
                                if (this.Velocity.Y < 0f && num7 > 0f)
                                {
                                    this.Velocity.Y = this.Velocity.Y + num5;
                                }
                            }
                            else
                            {
                                if (this.Velocity.Y > num7)
                                {
                                    this.Velocity.Y = this.Velocity.Y - num5;
                                    if (this.Velocity.Y > 0f && num7 < 0f)
                                    {
                                        this.Velocity.Y = this.Velocity.Y - num5;
                                    }
                                }
                            }
                            if (Main.myPlayer == this.Owner)
                            {
                                Rectangle rectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.width, this.height);
                                if(Main.players[Owner].Intersects(rectangle))
                                {
                                    this.Kill();
                                }
                            }
                        }
                        this.rotation += 0.4f * (float)this.direction;
                        return;
                    }
                    if (this.aiStyle == 4)
                    {
                        this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 1.57f;
                        if (this.ai[0] == 0f)
                        {
                            this.alpha -= 50;
                            if (this.alpha <= 0)
                            {
                                this.alpha = 0;
                                this.ai[0] = 1f;
                                if (this.ai[1] == 0f)
                                {
                                    this.ai[1] += 1f;
                                    this.Position += this.Velocity * 1f;
                                }
                                if (this.type == ProjectileType.VILETHORN && Main.myPlayer == this.Owner)
                                {
                                    int num9 = (int)this.type;
                                    if (this.ai[1] >= 6f)
                                    {
                                        num9++;
                                    }
                                    int num10 = Projectile.NewProjectile(this.Position.X + this.Velocity.X + (float)(this.width / 2), this.Position.Y + this.Velocity.Y + (float)(this.height / 2), this.Velocity.X, this.Velocity.Y, (ProjectileType)Enum.ToObject(typeof(ProjectileType),num9), this.damage, this.knockBack, this.Owner);
                                    Main.projectile[num10].damage = this.damage;
                                    Main.projectile[num10].ai[1] = this.ai[1] + 1f;
                                    NetMessage.SendData(27, -1, -1, "", num10);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (this.alpha < 170 && this.alpha + 5 >= 170)
                            {
                                Color newColor;
                                for (int j = 0; j < 3; j++)
                                {
                                    Vector2 arg_AF5_0 = this.Position;
                                    int arg_AF5_1 = this.width;
                                    int arg_AF5_2 = this.height;
                                    int arg_AF5_3 = 18;
                                    float arg_AF5_4 = this.Velocity.X * 0.025f;
                                    float arg_AF5_5 = this.Velocity.Y * 0.025f;
                                    int arg_AF5_6 = 170;
                                    newColor = default(Color);
                                    Dust.NewDust(arg_AF5_0, arg_AF5_1, arg_AF5_2, arg_AF5_3, arg_AF5_4, arg_AF5_5, arg_AF5_6, newColor, 1.2f);
                                }
                                Vector2 arg_B38_0 = this.Position;
                                int arg_B38_1 = this.width;
                                int arg_B38_2 = this.height;
                                int arg_B38_3 = 14;
                                float arg_B38_4 = 0f;
                                float arg_B38_5 = 0f;
                                int arg_B38_6 = 170;
                                newColor = default(Color);
                                Dust.NewDust(arg_B38_0, arg_B38_1, arg_B38_2, arg_B38_3, arg_B38_4, arg_B38_5, arg_B38_6, newColor, 1.1f);
                            }
                            this.alpha += 5;
                            if (this.alpha >= 255)
                            {
                                this.Kill();
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (this.aiStyle == 5)
                        {
                            if (this.soundDelay == 0)
                            {
                                this.soundDelay = 20 + Main.rand.Next(40);
                            }
                            if (this.ai[0] == 0f)
                            {
                                this.ai[0] = 1f;
                            }
                            this.alpha += (int)(25f * this.ai[0]);
                            if (this.alpha > 200)
                            {
                                this.alpha = 200;
                                this.ai[0] = -1f;
                            }
                            if (this.alpha < 0)
                            {
                                this.alpha = 0;
                                this.ai[0] = 1f;
                            }
                            this.rotation += (Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y)) * 0.01f * (float)this.direction;
                            if (Main.rand.Next(10) == 0)
                            {
                                Vector2 arg_CBA_0 = this.Position;
                                int arg_CBA_1 = this.width;
                                int arg_CBA_2 = this.height;
                                int arg_CBA_3 = 15;
                                float arg_CBA_4 = this.Velocity.X * 0.5f;
                                float arg_CBA_5 = this.Velocity.Y * 0.5f;
                                int arg_CBA_6 = 150;
                                Color newColor = default(Color);
                                Dust.NewDust(arg_CBA_0, arg_CBA_1, arg_CBA_2, arg_CBA_3, arg_CBA_4, arg_CBA_5, arg_CBA_6, newColor, 1.2f);
                            }
                            if (Main.rand.Next(20) == 0)
                            {
                                Gore.NewGore(this.Position, new Vector2(this.Velocity.X * 0.2f, this.Velocity.Y * 0.2f), Main.rand.Next(16, 18));
                                return;
                            }
                        }
                        else
                        {
                            if (this.aiStyle == 6)
                            {
                                this.Velocity *= 0.95f;
                                this.ai[0] += 1f;
                                if (this.ai[0] == 180f)
                                {
                                    this.Kill();
                                }
                                if (this.ai[1] == 0f)
                                {
                                    this.ai[1] = 1f;
                                    for (int k = 0; k < 30; k++)
                                    {
                                        Vector2 arg_DCA_0 = this.Position;
                                        int arg_DCA_1 = this.width;
                                        int arg_DCA_2 = this.height;
                                        int arg_DCA_3 = 10 + (int)this.type;
                                        float arg_DCA_4 = this.Velocity.X;
                                        float arg_DCA_5 = this.Velocity.Y;
                                        int arg_DCA_6 = 50;
                                        Color newColor = default(Color);
                                        Dust.NewDust(arg_DCA_0, arg_DCA_1, arg_DCA_2, arg_DCA_3, arg_DCA_4, arg_DCA_5, arg_DCA_6, newColor, 1f);
                                    }
                                }
                                if (this.type == ProjectileType.POWDER_PURIFICATION)
                                {
                                    int num11 = (int)(this.Position.X / 16f) - 1;
                                    int num12 = (int)((this.Position.X + (float)this.width) / 16f) + 2;
                                    int num13 = (int)(this.Position.Y / 16f) - 1;
                                    int num14 = (int)((this.Position.Y + (float)this.height) / 16f) + 2;
                                    if (num11 < 0)
                                    {
                                        num11 = 0;
                                    }
                                    if (num12 > Main.maxTilesX)
                                    {
                                        num12 = Main.maxTilesX;
                                    }
                                    if (num13 < 0)
                                    {
                                        num13 = 0;
                                    }
                                    if (num14 > Main.maxTilesY)
                                    {
                                        num14 = Main.maxTilesY;
                                    }
                                    for (int l = num11; l < num12; l++)
                                    {
                                        for (int m = num13; m < num14; m++)
                                        {
                                            Vector2 vector2;
                                            vector2.X = (float)(l * 16);
                                            vector2.Y = (float)(m * 16);
                                            if (this.Position.X + (float)this.width > vector2.X && this.Position.X < vector2.X + 16f && this.Position.Y + (float)this.height > vector2.Y && this.Position.Y < vector2.Y + 16f && Main.myPlayer == this.Owner && Main.tile[l, m].Active)
                                            {
                                                if (Main.tile[l, m].type == 23)
                                                {
                                                    Main.tile[l, m].type = 2;
                                                    WorldGen.SquareTileFrame(l, m, true);
                                                }
                                                if (Main.tile[l, m].type == 25)
                                                {
                                                    Main.tile[l, m].type = 1;
                                                    WorldGen.SquareTileFrame(l, m, true);
                                                }
                                            }
                                        }
                                    }
                                    return;
                                }
                            }
                            else
                            {
                                if (this.aiStyle == 7)
                                {
                                    if (Main.players[this.Owner].dead)
                                    {
                                        this.Kill();
                                        return;
                                    }
                                    Vector2 vector3 = new Vector2(this.Position.X + (float)this.width * 0.5f, this.Position.Y + (float)this.height * 0.5f);
                                    float num15 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].width / 2) - vector3.X;
                                    float num16 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].height / 2) - vector3.Y;
                                    float num17 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
                                    this.rotation = (float)Math.Atan2((double)num16, (double)num15) - 1.57f;
                                    if (this.ai[0] == 0f)
                                    {
                                        if ((num17 > 300f && this.type == ProjectileType.HOOK) || (num17 > 400f && this.type == ProjectileType.WHIP_IVY))
                                        {
                                            this.ai[0] = 1f;
                                        }
                                        int num18 = (int)(this.Position.X / 16f) - 1;
                                        int num19 = (int)((this.Position.X + (float)this.width) / 16f) + 2;
                                        int num20 = (int)(this.Position.Y / 16f) - 1;
                                        int num21 = (int)((this.Position.Y + (float)this.height) / 16f) + 2;
                                        if (num18 < 0)
                                        {
                                            num18 = 0;
                                        }
                                        if (num19 > Main.maxTilesX)
                                        {
                                            num19 = Main.maxTilesX;
                                        }
                                        if (num20 < 0)
                                        {
                                            num20 = 0;
                                        }
                                        if (num21 > Main.maxTilesY)
                                        {
                                            num21 = Main.maxTilesY;
                                        }
                                        for (int n = num18; n < num19; n++)
                                        {
                                            int num22 = num20;
                                            while (num22 < num21)
                                            {
                                                if (Main.tile[n, num22] == null)
                                                {
                                                    Main.tile[n, num22] = new Tile();
                                                }
                                                Vector2 vector4;
                                                vector4.X = (float)(n * 16);
                                                vector4.Y = (float)(num22 * 16);
                                                if (this.Position.X + (float)this.width > vector4.X && this.Position.X < vector4.X + 16f && this.Position.Y + (float)this.height > vector4.Y && this.Position.Y < vector4.Y + 16f && Main.tile[n, num22].Active && Main.tileSolid[(int)Main.tile[n, num22].type])
                                                {
                                                    if (Main.players[this.Owner].grapCount < 10)
                                                    {
                                                        Main.players[this.Owner].grappling[Main.players[this.Owner].grapCount] = this.whoAmI;
                                                        Main.players[this.Owner].grapCount++;
                                                    }
                                                    if (Main.myPlayer == this.Owner)
                                                    {
                                                        int num23 = 0;
                                                        int num24 = -1;
                                                        int num25 = 100000;
                                                        for (int num26 = 0; num26 < 1000; num26++)
                                                        {
                                                            if (Main.projectile[num26].Active && Main.projectile[num26].Owner == this.Owner && Main.projectile[num26].aiStyle == 7)
                                                            {
                                                                if (Main.projectile[num26].timeLeft < num25)
                                                                {
                                                                    num24 = num26;
                                                                    num25 = Main.projectile[num26].timeLeft;
                                                                }
                                                                num23++;
                                                            }
                                                        }
                                                        if (num23 > 3)
                                                        {
                                                            Main.projectile[num24].Kill();
                                                        }
                                                    }
                                                    WorldGen.KillTile(n, num22, true, true, false);
                                                    this.Velocity.X = 0f;
                                                    this.Velocity.Y = 0f;
                                                    this.ai[0] = 2f;
                                                    this.Position.X = (float)(n * 16 + 8 - this.width / 2);
                                                    this.Position.Y = (float)(num22 * 16 + 8 - this.height / 2);
                                                    this.damage = 0;
                                                    this.netUpdate = true;
                                                    if (Main.myPlayer == this.Owner)
                                                    {
                                                        NetMessage.SendData(13, -1, -1, "", this.Owner);
                                                        break;
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                                    num22++;
                                                }
                                            }
                                            if (this.ai[0] == 2f)
                                            {
                                                return;
                                            }
                                        }
                                        return;
                                    }
                                    if (this.ai[0] == 1f)
                                    {
                                        float num27 = 11f;
                                        if (this.type == ProjectileType.WHIP_IVY)
                                        {
                                            num27 = 15f;
                                        }
                                        if (num17 < 24f)
                                        {
                                            this.Kill();
                                        }
                                        num17 = num27 / num17;
                                        num15 *= num17;
                                        num16 *= num17;
                                        this.Velocity.X = num15;
                                        this.Velocity.Y = num16;
                                        return;
                                    }
                                    if (this.ai[0] == 2f)
                                    {
                                        int num28 = (int)(this.Position.X / 16f) - 1;
                                        int num29 = (int)((this.Position.X + (float)this.width) / 16f) + 2;
                                        int num30 = (int)(this.Position.Y / 16f) - 1;
                                        int num31 = (int)((this.Position.Y + (float)this.height) / 16f) + 2;
                                        if (num28 < 0)
                                        {
                                            num28 = 0;
                                        }
                                        if (num29 > Main.maxTilesX)
                                        {
                                            num29 = Main.maxTilesX;
                                        }
                                        if (num30 < 0)
                                        {
                                            num30 = 0;
                                        }
                                        if (num31 > Main.maxTilesY)
                                        {
                                            num31 = Main.maxTilesY;
                                        }
                                        bool flag = true;
                                        for (int num32 = num28; num32 < num29; num32++)
                                        {
                                            for (int num33 = num30; num33 < num31; num33++)
                                            {
                                                if (Main.tile[num32, num33] == null)
                                                {
                                                    Main.tile[num32, num33] = new Tile();
                                                }
                                                Vector2 vector5;
                                                vector5.X = (float)(num32 * 16);
                                                vector5.Y = (float)(num33 * 16);
                                                if (this.Position.X + (float)(this.width / 2) > vector5.X && this.Position.X + (float)(this.width / 2) < vector5.X + 16f && this.Position.Y + (float)(this.height / 2) > vector5.Y && this.Position.Y + (float)(this.height / 2) < vector5.Y + 16f && Main.tile[num32, num33].Active && Main.tileSolid[(int)Main.tile[num32, num33].type])
                                                {
                                                    flag = false;
                                                }
                                            }
                                        }
                                        if (flag)
                                        {
                                            this.ai[0] = 1f;
                                            return;
                                        }
                                        if (Main.players[this.Owner].grapCount < 10)
                                        {
                                            Main.players[this.Owner].grappling[Main.players[this.Owner].grapCount] = this.whoAmI;
                                            Main.players[this.Owner].grapCount++;
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    if (this.aiStyle == 8)
                                    {
                                        if (this.type == ProjectileType.BOLT_WATER)
                                        {
                                            Vector2 arg_17E5_0 = new Vector2(this.Position.X + this.Velocity.X, this.Position.Y + this.Velocity.Y);
                                            int arg_17E5_1 = this.width;
                                            int arg_17E5_2 = this.height;
                                            int arg_17E5_3 = 29;
                                            float arg_17E5_4 = this.Velocity.X;
                                            float arg_17E5_5 = this.Velocity.Y;
                                            int arg_17E5_6 = 100;
                                            Color newColor = default(Color);
                                            int num34 = Dust.NewDust(arg_17E5_0, arg_17E5_1, arg_17E5_2, arg_17E5_3, arg_17E5_4, arg_17E5_5, arg_17E5_6, newColor, 3f);
                                            Main.dust[num34].noGravity = true;
                                            Vector2 arg_184A_0 = new Vector2(this.Position.X, this.Position.Y);
                                            int arg_184A_1 = this.width;
                                            int arg_184A_2 = this.height;
                                            int arg_184A_3 = 29;
                                            float arg_184A_4 = this.Velocity.X;
                                            float arg_184A_5 = this.Velocity.Y;
                                            int arg_184A_6 = 100;
                                            newColor = default(Color);
                                            num34 = Dust.NewDust(arg_184A_0, arg_184A_1, arg_184A_2, arg_184A_3, arg_184A_4, arg_184A_5, arg_184A_6, newColor, 1.5f);
                                        }
                                        else
                                        {
                                            for (int num35 = 0; num35 < 2; num35++)
                                            {
                                                Vector2 arg_18B9_0 = new Vector2(this.Position.X, this.Position.Y);
                                                int arg_18B9_1 = this.width;
                                                int arg_18B9_2 = this.height;
                                                int arg_18B9_3 = 6;
                                                float arg_18B9_4 = this.Velocity.X * 0.2f;
                                                float arg_18B9_5 = this.Velocity.Y * 0.2f;
                                                int arg_18B9_6 = 100;
                                                Color newColor = default(Color);
                                                int num36 = Dust.NewDust(arg_18B9_0, arg_18B9_1, arg_18B9_2, arg_18B9_3, arg_18B9_4, arg_18B9_5, arg_18B9_6, newColor, 2f);
                                                Main.dust[num36].noGravity = true;
                                                Dust expr_18DB_cp_0 = Main.dust[num36];
                                                expr_18DB_cp_0.velocity.X = expr_18DB_cp_0.velocity.X * 0.3f;
                                                Dust expr_18F9_cp_0 = Main.dust[num36];
                                                expr_18F9_cp_0.velocity.Y = expr_18F9_cp_0.velocity.Y * 0.3f;
                                            }
                                        }
                                        if (this.type != ProjectileType.BOLT_WATER)
                                        {
                                            this.ai[1] += 1f;
                                        }
                                        if (this.ai[1] >= 20f)
                                        {
                                            this.Velocity.Y = this.Velocity.Y + 0.2f;
                                        }
                                        this.rotation += 0.3f * (float)this.direction;
                                        if (this.Velocity.Y > 16f)
                                        {
                                            this.Velocity.Y = 16f;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (this.aiStyle == 9)
                                        {
                                            if (this.type == ProjectileType.FLAMELASH)
                                            {
                                                Vector2 arg_1A1A_0 = new Vector2(this.Position.X, this.Position.Y);
                                                int arg_1A1A_1 = this.width;
                                                int arg_1A1A_2 = this.height;
                                                int arg_1A1A_3 = 6;
                                                float arg_1A1A_4 = this.Velocity.X * 0.2f;
                                                float arg_1A1A_5 = this.Velocity.Y * 0.2f;
                                                int arg_1A1A_6 = 100;
                                                Color newColor = default(Color);
                                                int num37 = Dust.NewDust(arg_1A1A_0, arg_1A1A_1, arg_1A1A_2, arg_1A1A_3, arg_1A1A_4, arg_1A1A_5, arg_1A1A_6, newColor, 3.5f);
                                                Main.dust[num37].noGravity = true;
                                                Dust expr_1A37 = Main.dust[num37];
                                                expr_1A37.velocity *= 1.4f;
                                                Vector2 arg_1AA7_0 = new Vector2(this.Position.X, this.Position.Y);
                                                int arg_1AA7_1 = this.width;
                                                int arg_1AA7_2 = this.height;
                                                int arg_1AA7_3 = 6;
                                                float arg_1AA7_4 = this.Velocity.X * 0.2f;
                                                float arg_1AA7_5 = this.Velocity.Y * 0.2f;
                                                int arg_1AA7_6 = 100;
                                                newColor = default(Color);
                                                num37 = Dust.NewDust(arg_1AA7_0, arg_1AA7_1, arg_1AA7_2, arg_1AA7_3, arg_1AA7_4, arg_1AA7_5, arg_1AA7_6, newColor, 1.5f);
                                            }
                                            else
                                            {
                                                if (this.soundDelay == 0 && Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y) > 2f)
                                                {
                                                    this.soundDelay = 10;
                                                }
                                                Vector2 arg_1B4F_0 = new Vector2(this.Position.X, this.Position.Y);
                                                int arg_1B4F_1 = this.width;
                                                int arg_1B4F_2 = this.height;
                                                int arg_1B4F_3 = 15;
                                                float arg_1B4F_4 = 0f;
                                                float arg_1B4F_5 = 0f;
                                                int arg_1B4F_6 = 100;
                                                Color newColor = default(Color);
                                                int num38 = Dust.NewDust(arg_1B4F_0, arg_1B4F_1, arg_1B4F_2, arg_1B4F_3, arg_1B4F_4, arg_1B4F_5, arg_1B4F_6, newColor, 2f);
                                                Dust expr_1B5E = Main.dust[num38];
                                                expr_1B5E.velocity *= 0.3f;
                                                Main.dust[num38].position.X = this.Position.X + (float)(this.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
                                                Main.dust[num38].position.Y = this.Position.Y + (float)(this.height / 2) + (float)Main.rand.Next(-4, 5);
                                                Main.dust[num38].noGravity = true;
                                            }
                                            if (Main.myPlayer == this.Owner && this.ai[0] == 0f)
                                            {
                                                if (!Main.players[this.Owner].channel)
                                                {
                                                    this.Kill();
                                                }
                                            }
                                            if (this.type == ProjectileType.FLAMELASH)
                                            {
                                                this.rotation += 0.3f * (float)this.direction;
                                            }
                                            else
                                            {
                                                if (this.Velocity.X != 0f || this.Velocity.Y != 0f)
                                                {
                                                    this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) - 2.355f;
                                                }
                                            }
                                            if (this.Velocity.Y > 16f)
                                            {
                                                this.Velocity.Y = 16f;
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            if (this.aiStyle == 10)
                                            {
                                                if (this.type == ProjectileType.BALL_SAND_DROP && this.ai[0] != 2f)
                                                {
                                                    if (Main.rand.Next(2) == 0)
                                                    {
                                                        Vector2 arg_1E9B_0 = new Vector2(this.Position.X, this.Position.Y);
                                                        int arg_1E9B_1 = this.width;
                                                        int arg_1E9B_2 = this.height;
                                                        int arg_1E9B_3 = 32;
                                                        float arg_1E9B_4 = 0f;
                                                        float arg_1E9B_5 = this.Velocity.Y / 2f;
                                                        int arg_1E9B_6 = 0;
                                                        Color newColor = default(Color);
                                                        int num43 = Dust.NewDust(arg_1E9B_0, arg_1E9B_1, arg_1E9B_2, arg_1E9B_3, arg_1E9B_4, arg_1E9B_5, arg_1E9B_6, newColor, 1f);
                                                        Dust expr_1EAF_cp_0 = Main.dust[num43];
                                                        expr_1EAF_cp_0.velocity.X = expr_1EAF_cp_0.velocity.X * 0.4f;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.type == ProjectileType.BALL_MUD)
                                                    {
                                                        if (Main.rand.Next(2) == 0)
                                                        {
                                                            Vector2 arg_1F31_0 = new Vector2(this.Position.X, this.Position.Y);
                                                            int arg_1F31_1 = this.width;
                                                            int arg_1F31_2 = this.height;
                                                            int arg_1F31_3 = 38;
                                                            float arg_1F31_4 = 0f;
                                                            float arg_1F31_5 = this.Velocity.Y / 2f;
                                                            int arg_1F31_6 = 0;
                                                            Color newColor = default(Color);
                                                            int num44 = Dust.NewDust(arg_1F31_0, arg_1F31_1, arg_1F31_2, arg_1F31_3, arg_1F31_4, arg_1F31_5, arg_1F31_6, newColor, 1f);
                                                            Dust expr_1F45_cp_0 = Main.dust[num44];
                                                            expr_1F45_cp_0.velocity.X = expr_1F45_cp_0.velocity.X * 0.4f;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (this.type == ProjectileType.BALL_ASH)
                                                        {
                                                            if (Main.rand.Next(2) == 0)
                                                            {
                                                                Vector2 arg_1FC7_0 = new Vector2(this.Position.X, this.Position.Y);
                                                                int arg_1FC7_1 = this.width;
                                                                int arg_1FC7_2 = this.height;
                                                                int arg_1FC7_3 = 36;
                                                                float arg_1FC7_4 = 0f;
                                                                float arg_1FC7_5 = this.Velocity.Y / 2f;
                                                                int arg_1FC7_6 = 0;
                                                                Color newColor = default(Color);
                                                                int num45 = Dust.NewDust(arg_1FC7_0, arg_1FC7_1, arg_1FC7_2, arg_1FC7_3, arg_1FC7_4, arg_1FC7_5, arg_1FC7_6, newColor, 1f);
                                                                Dust expr_1FD6 = Main.dust[num45];
                                                                expr_1FD6.velocity *= 0.4f;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (this.type == ProjectileType.BALL_SAND_GUN || this.type == ProjectileType.BALL_SAND_DROP)
                                                            {
                                                                if (Main.rand.Next(2) == 0)
                                                                {
                                                                    Vector2 arg_2057_0 = new Vector2(this.Position.X, this.Position.Y);
                                                                    int arg_2057_1 = this.width;
                                                                    int arg_2057_2 = this.height;
                                                                    int arg_2057_3 = 32;
                                                                    float arg_2057_4 = 0f;
                                                                    float arg_2057_5 = 0f;
                                                                    int arg_2057_6 = 0;
                                                                    Color newColor = default(Color);
                                                                    int num46 = Dust.NewDust(arg_2057_0, arg_2057_1, arg_2057_2, arg_2057_3, arg_2057_4, arg_2057_5, arg_2057_6, newColor, 1f);
                                                                    Dust expr_206B_cp_0 = Main.dust[num46];
                                                                    expr_206B_cp_0.velocity.X = expr_206B_cp_0.velocity.X * 0.4f;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (Main.rand.Next(20) == 0)
                                                                {
                                                                    Vector2 arg_20CE_0 = new Vector2(this.Position.X, this.Position.Y);
                                                                    int arg_20CE_1 = this.width;
                                                                    int arg_20CE_2 = this.height;
                                                                    int arg_20CE_3 = 0;
                                                                    float arg_20CE_4 = 0f;
                                                                    float arg_20CE_5 = 0f;
                                                                    int arg_20CE_6 = 0;
                                                                    Color newColor = default(Color);
                                                                    Dust.NewDust(arg_20CE_0, arg_20CE_1, arg_20CE_2, arg_20CE_3, arg_20CE_4, arg_20CE_5, arg_20CE_6, newColor, 1f);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if (Main.myPlayer == this.Owner && this.ai[0] == 0f)
                                                {
                                                    if (!Main.players[this.Owner].channel)
                                                    {
                                                        this.ai[0] = 1f;
                                                        this.netUpdate = true;
                                                    }
                                                }
                                                if (this.ai[0] == 1f)
                                                {
                                                    if (this.type == ProjectileType.BALL_SAND_GUN)
                                                    {
                                                        this.ai[1] += 1f;
                                                        if (this.ai[1] >= 15f)
                                                        {
                                                            this.ai[1] = 15f;
                                                            this.Velocity.Y = this.Velocity.Y + 0.2f;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        this.Velocity.Y = this.Velocity.Y + 0.41f;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.ai[0] == 2f)
                                                    {
                                                        this.Velocity.Y = this.Velocity.Y + 0.2f;
                                                        if ((double)this.Velocity.X < -0.04)
                                                        {
                                                            this.Velocity.X = this.Velocity.X + 0.04f;
                                                        }
                                                        else
                                                        {
                                                            if ((double)this.Velocity.X > 0.04)
                                                            {
                                                                this.Velocity.X = this.Velocity.X - 0.04f;
                                                            }
                                                            else
                                                            {
                                                                this.Velocity.X = 0f;
                                                            }
                                                        }
                                                    }
                                                }
                                                this.rotation += 0.1f;
                                                if (this.Velocity.Y > 10f)
                                                {
                                                    this.Velocity.Y = 10f;
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                if (this.aiStyle == 11)
                                                {
                                                    this.rotation += 0.02f;
                                                    if (Main.myPlayer == this.Owner)
                                                    {
                                                        if (Main.players[this.Owner].dead)
                                                        {
                                                            this.Kill();
                                                            return;
                                                        }
                                                        float num51 = 4f;
                                                        Vector2 vector8 = new Vector2(this.Position.X + (float)this.width * 0.5f, this.Position.Y + (float)this.height * 0.5f);
                                                        float num52 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].width / 2) - vector8.X;
                                                        float num53 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].height / 2) - vector8.Y;
                                                        float num54 = (float)Math.Sqrt((double)(num52 * num52 + num53 * num53));
                                                        num54 = (float)Math.Sqrt((double)(num52 * num52 + num53 * num53));
                                                        if (num54 > (float)Main.screenWidth)
                                                        {
                                                            this.Position.X = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].width / 2) - (float)(this.width / 2);
                                                            this.Position.Y = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].height / 2) - (float)(this.height / 2);
                                                            return;
                                                        }
                                                        if (num54 > 64f)
                                                        {
                                                            num54 = num51 / num54;
                                                            num52 *= num54;
                                                            num53 *= num54;
                                                            if (num52 != this.Velocity.X || num53 != this.Velocity.Y)
                                                            {
                                                                this.netUpdate = true;
                                                            }
                                                            this.Velocity.X = num52;
                                                            this.Velocity.Y = num53;
                                                            return;
                                                        }
                                                        if (this.Velocity.X != 0f || this.Velocity.Y != 0f)
                                                        {
                                                            this.netUpdate = true;
                                                        }
                                                        this.Velocity.X = 0f;
                                                        this.Velocity.Y = 0f;
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.aiStyle == 12)
                                                    {
                                                        this.scale -= 0.05f;
                                                        if (this.scale <= 0f)
                                                        {
                                                            this.Kill();
                                                        }
                                                        if (this.ai[0] > 4f)
                                                        {
                                                            this.alpha = 150;
                                                            this.light = 0.8f;
                                                            Vector2 arg_26C3_0 = new Vector2(this.Position.X, this.Position.Y);
                                                            int arg_26C3_1 = this.width;
                                                            int arg_26C3_2 = this.height;
                                                            int arg_26C3_3 = 29;
                                                            float arg_26C3_4 = this.Velocity.X;
                                                            float arg_26C3_5 = this.Velocity.Y;
                                                            int arg_26C3_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num55 = Dust.NewDust(arg_26C3_0, arg_26C3_1, arg_26C3_2, arg_26C3_3, arg_26C3_4, arg_26C3_5, arg_26C3_6, newColor, 2.5f);
                                                            Main.dust[num55].noGravity = true;
                                                            Vector2 arg_2728_0 = new Vector2(this.Position.X, this.Position.Y);
                                                            int arg_2728_1 = this.width;
                                                            int arg_2728_2 = this.height;
                                                            int arg_2728_3 = 29;
                                                            float arg_2728_4 = this.Velocity.X;
                                                            float arg_2728_5 = this.Velocity.Y;
                                                            int arg_2728_6 = 100;
                                                            newColor = default(Color);
                                                            Dust.NewDust(arg_2728_0, arg_2728_1, arg_2728_2, arg_2728_3, arg_2728_4, arg_2728_5, arg_2728_6, newColor, 1.5f);
                                                        }
                                                        else
                                                        {
                                                            this.ai[0] += 1f;
                                                        }
                                                        this.rotation += 0.3f * (float)this.direction;
                                                        return;
                                                    }
                                                    if (this.aiStyle == 13)
                                                    {
                                                        if (Main.players[this.Owner].dead)
                                                        {
                                                            this.Kill();
                                                            return;
                                                        }
                                                        Main.players[this.Owner].itemAnimation = 5;
                                                        Main.players[this.Owner].itemTime = 5;
                                                        if (this.Position.X + (float)(this.width / 2) > Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].width / 2))
                                                        {
                                                            Main.players[this.Owner].direction = 1;
                                                        }
                                                        else
                                                        {
                                                            Main.players[this.Owner].direction = -1;
                                                        }
                                                        Vector2 vector9 = new Vector2(this.Position.X + (float)this.width * 0.5f, this.Position.Y + (float)this.height * 0.5f);
                                                        float num56 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].width / 2) - vector9.X;
                                                        float num57 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].height / 2) - vector9.Y;
                                                        float num58 = (float)Math.Sqrt((double)(num56 * num56 + num57 * num57));
                                                        if (this.ai[0] == 0f)
                                                        {
                                                            if (num58 > 600f)
                                                            {
                                                                this.ai[0] = 1f;
                                                            }
                                                            this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 1.57f;
                                                            this.ai[1] += 1f;
                                                            if (this.ai[1] > 2f)
                                                            {
                                                                this.alpha = 0;
                                                            }
                                                            if (this.ai[1] >= 10f)
                                                            {
                                                                this.ai[1] = 15f;
                                                                this.Velocity.Y = this.Velocity.Y + 0.3f;
                                                                return;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (this.ai[0] == 1f)
                                                            {
                                                                this.tileCollide = false;
                                                                this.rotation = (float)Math.Atan2((double)num57, (double)num56) - 1.57f;
                                                                float num59 = 11f;
                                                                if (num58 < 50f)
                                                                {
                                                                    this.Kill();
                                                                }
                                                                num58 = num59 / num58;
                                                                num56 *= num58;
                                                                num57 *= num58;
                                                                this.Velocity.X = num56;
                                                                this.Velocity.Y = num57;
                                                                return;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (this.aiStyle == 14)
                                                        {
                                                            if (this.type == ProjectileType.GLOWSTICK_STICKY)
                                                            {
                                                                try
                                                                {
                                                                    Vector2 vector10 = Collision.TileCollision(this.Position, this.Velocity, this.width, this.height, false, false);
                                                                    bool flag1 = this.Velocity != vector10;
                                                                    int num66 = ((int)(this.Position.X / 16f)) - 1;
                                                                    int num67 = ((int)((this.Position.X + this.width) / 16f)) + 2;
                                                                    int num68 = ((int)(this.Position.Y / 16f)) - 1;
                                                                    int num69 = ((int)((this.Position.Y + this.height) / 16f)) + 2;
                                                                    if (num66 < 0)
                                                                    {
                                                                        num66 = 0;
                                                                    }
                                                                    if (num67 > Main.maxTilesX)
                                                                    {
                                                                        num67 = Main.maxTilesX;
                                                                    }
                                                                    if (num68 < 0)
                                                                    {
                                                                        num68 = 0;
                                                                    }
                                                                    if (num69 > Main.maxTilesY)
                                                                    {
                                                                        num69 = Main.maxTilesY;
                                                                    }
                                                                    for (int num70 = num66; num70 < num67; num70++)
                                                                    {
                                                                        for (int num71 = num68; num71 < num69; num71++)
                                                                        {
                                                                            if (((Main.tile[num70, num71] != null) && Main.tile[num70, num71].Active) && (Main.tileSolid[Main.tile[num70, num71].type] || (Main.tileSolidTop[Main.tile[num70, num71].type] && (Main.tile[num70, num71].frameY == 0))))
                                                                            {
                                                                                Vector2 vector11;
                                                                                vector11.X = num70 * 0x10;
                                                                                vector11.Y = num71 * 0x10;
                                                                                if ((((this.Position.X + this.width) > vector11.X) && (this.Position.X < (vector11.X + 16f))) && (((this.Position.Y + this.height) > vector11.Y) && (this.Position.Y < (vector11.Y + 16f))))
                                                                                {
                                                                                    this.Velocity.X = 0f;
                                                                                    this.Velocity.Y = -0.2f;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                catch
                                                                {
                                                                }
                                                            }

                                                            this.ai[0] += 1f;
                                                            if (this.ai[0] > 5f)
                                                            {
                                                                this.ai[0] = 5f;
                                                                if (this.Velocity.Y == 0f && this.Velocity.X != 0f)
                                                                {
                                                                    this.Velocity.X = this.Velocity.X * 0.97f;
                                                                    if ((double)this.Velocity.X > -0.01 && (double)this.Velocity.X < 0.01)
                                                                    {
                                                                        this.Velocity.X = 0f;
                                                                        this.netUpdate = true;
                                                                    }
                                                                }
                                                                this.Velocity.Y = this.Velocity.Y + 0.2f;
                                                            }
                                                            this.rotation += this.Velocity.X * 0.1f;
                                                            return;
                                                        }
                                                        if (this.aiStyle == 15)
                                                        {
                                                            if (this.type == ProjectileType.BALL_O_HURT)
                                                            {
                                                                if (Main.rand.Next(15) == 0)
                                                                {
                                                                    Vector2 arg_2D89_0 = this.Position;
                                                                    int arg_2D89_1 = this.width;
                                                                    int arg_2D89_2 = this.height;
                                                                    int arg_2D89_3 = 14;
                                                                    float arg_2D89_4 = 0f;
                                                                    float arg_2D89_5 = 0f;
                                                                    int arg_2D89_6 = 150;
                                                                    Color newColor = default(Color);
                                                                    Dust.NewDust(arg_2D89_0, arg_2D89_1, arg_2D89_2, arg_2D89_3, arg_2D89_4, arg_2D89_5, arg_2D89_6, newColor, 1.3f);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (this.type == ProjectileType.BLUE_MOON)
                                                                {
                                                                    Vector2 arg_2DE8_0 = this.Position;
                                                                    int arg_2DE8_1 = this.width;
                                                                    int arg_2DE8_2 = this.height;
                                                                    int arg_2DE8_3 = 29;
                                                                    float arg_2DE8_4 = this.Velocity.X * 0.4f;
                                                                    float arg_2DE8_5 = this.Velocity.Y * 0.4f;
                                                                    int arg_2DE8_6 = 100;
                                                                    Color newColor = default(Color);
                                                                    int num66 = Dust.NewDust(arg_2DE8_0, arg_2DE8_1, arg_2DE8_2, arg_2DE8_3, arg_2DE8_4, arg_2DE8_5, arg_2DE8_6, newColor, 2.5f);
                                                                    Main.dust[num66].noGravity = true;
                                                                    Dust expr_2E0A_cp_0 = Main.dust[num66];
                                                                    expr_2E0A_cp_0.velocity.X = expr_2E0A_cp_0.velocity.X / 2f;
                                                                    Dust expr_2E28_cp_0 = Main.dust[num66];
                                                                    expr_2E28_cp_0.velocity.Y = expr_2E28_cp_0.velocity.Y / 2f;
                                                                }
                                                                else
                                                                {
                                                                    if (this.type == ProjectileType.SUNFURY)
                                                                    {
                                                                        Vector2 arg_2E91_0 = this.Position;
                                                                        int arg_2E91_1 = this.width;
                                                                        int arg_2E91_2 = this.height;
                                                                        int arg_2E91_3 = 6;
                                                                        float arg_2E91_4 = this.Velocity.X * 0.4f;
                                                                        float arg_2E91_5 = this.Velocity.Y * 0.4f;
                                                                        int arg_2E91_6 = 100;
                                                                        Color newColor = default(Color);
                                                                        int num67 = Dust.NewDust(arg_2E91_0, arg_2E91_1, arg_2E91_2, arg_2E91_3, arg_2E91_4, arg_2E91_5, arg_2E91_6, newColor, 3f);
                                                                        Main.dust[num67].noGravity = true;
                                                                        Dust expr_2EB3_cp_0 = Main.dust[num67];
                                                                        expr_2EB3_cp_0.velocity.X = expr_2EB3_cp_0.velocity.X * 2f;
                                                                        Dust expr_2ED1_cp_0 = Main.dust[num67];
                                                                        expr_2ED1_cp_0.velocity.Y = expr_2ED1_cp_0.velocity.Y * 2f;
                                                                    }
                                                                }
                                                            }
                                                            if (Main.players[this.Owner].dead)
                                                            {
                                                                this.Kill();
                                                                return;
                                                            }
                                                            Main.players[this.Owner].itemAnimation = 5;
                                                            Main.players[this.Owner].itemTime = 5;
                                                            if (this.Position.X + (float)(this.width / 2) > Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].width / 2))
                                                            {
                                                                Main.players[this.Owner].direction = 1;
                                                            }
                                                            else
                                                            {
                                                                Main.players[this.Owner].direction = -1;
                                                            }
                                                            Vector2 vector11 = new Vector2(this.Position.X + (float)this.width * 0.5f, this.Position.Y + (float)this.height * 0.5f);
                                                            float num68 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].width / 2) - vector11.X;
                                                            float num69 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].height / 2) - vector11.Y;
                                                            float num70 = (float)Math.Sqrt((double)(num68 * num68 + num69 * num69));
                                                            if (this.ai[0] == 0f)
                                                            {
                                                                this.tileCollide = true;
                                                                if (num70 > 300f)
                                                                {
                                                                    this.ai[0] = 1f;
                                                                }
                                                                else
                                                                {
                                                                    this.ai[1] += 1f;
                                                                    if (this.ai[1] > 2f)
                                                                    {
                                                                        this.alpha = 0;
                                                                    }
                                                                    if (this.ai[1] >= 5f)
                                                                    {
                                                                        this.ai[1] = 15f;
                                                                        this.Velocity.Y = this.Velocity.Y + 0.5f;
                                                                        this.Velocity.X = this.Velocity.X * 0.95f;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (this.ai[0] == 1f)
                                                                {
                                                                    this.tileCollide = false;
                                                                    float num71 = 11f;
                                                                    if (num70 < 20f)
                                                                    {
                                                                        this.Kill();
                                                                    }
                                                                    num70 = num71 / num70;
                                                                    num68 *= num70;
                                                                    num69 *= num70;
                                                                    this.Velocity.X = num68;
                                                                    this.Velocity.Y = num69;
                                                                }
                                                            }
                                                            this.rotation += this.Velocity.X * 0.03f;
                                                            return;
                                                        }
                                                        else
                                                        {
                                                            if (this.aiStyle == 16)
                                                            {
                                                                if (this.type == ProjectileType.BOMB_STICKY)
                                                                {
                                                                    try
                                                                    {
                                                                        int num72 = (int)(this.Position.X / 16f) - 1;
                                                                        int num73 = (int)((this.Position.X + (float)this.width) / 16f) + 2;
                                                                        int num74 = (int)(this.Position.Y / 16f) - 1;
                                                                        int num75 = (int)((this.Position.Y + (float)this.height) / 16f) + 2;
                                                                        if (num72 < 0)
                                                                        {
                                                                            num72 = 0;
                                                                        }
                                                                        if (num73 > Main.maxTilesX)
                                                                        {
                                                                            num73 = Main.maxTilesX;
                                                                        }
                                                                        if (num74 < 0)
                                                                        {
                                                                            num74 = 0;
                                                                        }
                                                                        if (num75 > Main.maxTilesY)
                                                                        {
                                                                            num75 = Main.maxTilesY;
                                                                        }
                                                                        for (int num76 = num72; num76 < num73; num76++)
                                                                        {
                                                                            for (int num77 = num74; num77 < num75; num77++)
                                                                            {
                                                                                if (Main.tile[num76, num77] != null && Main.tile[num76, num77].Active && (Main.tileSolid[(int)Main.tile[num76, num77].type] || (Main.tileSolidTop[(int)Main.tile[num76, num77].type] && Main.tile[num76, num77].frameY == 0)))
                                                                                {
                                                                                    Vector2 vector12;
                                                                                    vector12.X = (float)(num76 * 16);
                                                                                    vector12.Y = (float)(num77 * 16);
                                                                                    if (this.Position.X + (float)this.width - 4f > vector12.X && this.Position.X + 4f < vector12.X + 16f && this.Position.Y + (float)this.height - 4f > vector12.Y && this.Position.Y + 4f < vector12.Y + 16f)
                                                                                    {
                                                                                        this.Velocity.X = 0f;
                                                                                        this.Velocity.Y = -0.2f;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    catch
                                                                    {
                                                                    }
                                                                }
                                                                if (this.Owner == Main.myPlayer && this.timeLeft <= 3)
                                                                {
                                                                    this.ai[1] = 0f;
                                                                    this.alpha = 255;
                                                                    if (this.type == ProjectileType.BOMB || this.type == ProjectileType.BOMB_STICKY)
                                                                    {
                                                                        this.Position.X = this.Position.X + (float)(this.width / 2);
                                                                        this.Position.Y = this.Position.Y + (float)(this.height / 2);
                                                                        this.width = 128;
                                                                        this.height = 128;
                                                                        this.Position.X = this.Position.X - (float)(this.width / 2);
                                                                        this.Position.Y = this.Position.Y - (float)(this.height / 2);
                                                                        this.damage = 100;
                                                                        this.knockBack = 8f;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (this.type == ProjectileType.DYNAMITE)
                                                                        {
                                                                            this.Position.X = this.Position.X + (float)(this.width / 2);
                                                                            this.Position.Y = this.Position.Y + (float)(this.height / 2);
                                                                            this.width = 250;
                                                                            this.height = 250;
                                                                            this.Position.X = this.Position.X - (float)(this.width / 2);
                                                                            this.Position.Y = this.Position.Y - (float)(this.height / 2);
                                                                            this.damage = 250;
                                                                            this.knockBack = 10f;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (this.type == ProjectileType.GRENADE)
                                                                            {
                                                                                this.Position.X = this.Position.X + (float)(this.width / 2);
                                                                                this.Position.Y = this.Position.Y + (float)(this.height / 2);
                                                                                this.width = 128;
                                                                                this.height = 128;
                                                                                this.Position.X = this.Position.X - (float)(this.width / 2);
                                                                                this.Position.Y = this.Position.Y - (float)(this.height / 2);
                                                                                this.knockBack = 8f;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (this.type != ProjectileType.GRENADE && Main.rand.Next(4) == 0)
                                                                    {
                                                                        if (this.type != ProjectileType.GRENADE)
                                                                        {
                                                                            this.damage = 0;
                                                                        }
                                                                        Vector2 arg_3629_0 = new Vector2(this.Position.X, this.Position.Y);
                                                                        int arg_3629_1 = this.width;
                                                                        int arg_3629_2 = this.height;
                                                                        int arg_3629_3 = 6;
                                                                        float arg_3629_4 = 0f;
                                                                        float arg_3629_5 = 0f;
                                                                        int arg_3629_6 = 100;
                                                                        Color newColor = default(Color);
                                                                        Dust.NewDust(arg_3629_0, arg_3629_1, arg_3629_2, arg_3629_3, arg_3629_4, arg_3629_5, arg_3629_6, newColor, 1f);
                                                                    }
                                                                }
                                                                this.ai[0] += 1f;
                                                                if ((this.type == ProjectileType.GRENADE && this.ai[0] > 10f) || (this.type != ProjectileType.GRENADE && this.ai[0] > 5f))
                                                                {
                                                                    this.ai[0] = 10f;
                                                                    if (this.Velocity.Y == 0f && this.Velocity.X != 0f)
                                                                    {
                                                                        this.Velocity.X = this.Velocity.X * 0.97f;
                                                                        if (this.type == ProjectileType.DYNAMITE)
                                                                        {
                                                                            this.Velocity.X = this.Velocity.X * 0.99f;
                                                                        }
                                                                        if ((double)this.Velocity.X > -0.01 && (double)this.Velocity.X < 0.01)
                                                                        {
                                                                            this.Velocity.X = 0f;
                                                                            this.netUpdate = true;
                                                                        }
                                                                    }
                                                                    this.Velocity.Y = this.Velocity.Y + 0.2f;
                                                                }
                                                                this.rotation += this.Velocity.X * 0.1f;
                                                                return;
                                                            }
                                                            if (this.aiStyle == 17)
                                                            {
                                                                if (this.Velocity.Y == 0f)
                                                                {
                                                                    this.Velocity.X = this.Velocity.X * 0.98f;
                                                                }
                                                                this.rotation += this.Velocity.X * 0.1f;
                                                                this.Velocity.Y = this.Velocity.Y + 0.2f;
                                                                if (this.Owner == Main.myPlayer)
                                                                {
                                                                    int num78 = (int)((this.Position.X + (float)this.width) / 16f);
                                                                    int num79 = (int)((this.Position.Y + (float)this.height) / 16f);
                                                                    if (Main.tile[num78, num79] != null && !Main.tile[num78, num79].Active)
                                                                    {
                                                                        WorldGen.PlaceTile(num78, num79, 85, false, false, -1, 0);
                                                                        if (Main.tile[num78, num79].Active)
                                                                        {
                                                                            
                                                                            NetMessage.SendData(17, -1, -1, "", 1, (float)num78, (float)num79, 85f);

                                                                            int num80 = Sign.ReadSign(num78, num79);
                                                                            if (num80 >= 0)
                                                                            {
                                                                                //Need to check if this works :3
                                                                                //PlayerEditSignEvent playerEvent = new PlayerEditSignEvent();
                                                                                //playerEvent.Sender = Main.players[this.Owner];
                                                                                //playerEvent.Sign = Main.sign[num80];
                                                                                //playerEvent.Text = this.miscText;
                                                                                //playerEvent.isPlayer = false;
                                                                                //Program.server.getPluginManager().processHook(Hooks.PLAYER_EDITSIGN, playerEvent);
                                                                                //if (playerEvent.Cancelled)
                                                                                //{
                                                                                //    return;
                                                                                //}

                                                                                Sign.TextSign(num80, this.miscText);
                                                                            }
                                                                            this.Kill();
                                                                            return;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (this.aiStyle == 18)
                                                                {
                                                                    if (this.ai[1] == 0f && this.type == ProjectileType.SICKLE_DEMON)
                                                                    {
                                                                        this.ai[1] = 1f;
                                                                    }
                                                                    this.rotation += (float)this.direction * 0.8f;
                                                                    this.ai[0] += 1f;
                                                                    if (this.ai[0] >= 30f)
                                                                    {
                                                                        if (this.ai[0] < 100f)
                                                                        {
                                                                            this.Velocity *= 1.06f;
                                                                        }
                                                                        else
                                                                        {
                                                                            this.ai[0] = 200f;
                                                                        }
                                                                    }
                                                                    for (int num81 = 0; num81 < 2; num81++)
                                                                    {
                                                                        Vector2 arg_39CC_0 = new Vector2(this.Position.X, this.Position.Y);
                                                                        int arg_39CC_1 = this.width;
                                                                        int arg_39CC_2 = this.height;
                                                                        int arg_39CC_3 = 27;
                                                                        float arg_39CC_4 = 0f;
                                                                        float arg_39CC_5 = 0f;
                                                                        int arg_39CC_6 = 100;
                                                                        Color newColor = default(Color);
                                                                        int num82 = Dust.NewDust(arg_39CC_0, arg_39CC_1, arg_39CC_2, arg_39CC_3, arg_39CC_4, arg_39CC_5, arg_39CC_6, newColor, 1f);
                                                                        Main.dust[num82].noGravity = true;
                                                                    }
                                                                    return;
                                                                }
                                                                if (this.aiStyle == 19)
                                                                {
                                                                    this.direction = Main.players[this.Owner].direction;
                                                                    Main.players[this.Owner].heldProj = this.whoAmI;
                                                                    this.Position.X = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].width / 2) - (float)(this.width / 2);
                                                                    this.Position.Y = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].height / 2) - (float)(this.height / 2);
                                                                    if (this.type == ProjectileType.LANCE_DARK)
                                                                    {
                                                                        if (this.ai[0] == 0f)
                                                                        {
                                                                            this.ai[0] = 3f;
                                                                            this.netUpdate = true;
                                                                        }
                                                                        if (Main.players[this.Owner].itemAnimation < Main.players[this.Owner].inventory[Main.players[this.Owner].selectedItemIndex].UseAnimation / 3)
                                                                        {
                                                                            this.ai[0] -= 1.6f;
                                                                        }
                                                                        else
                                                                        {
                                                                            this.ai[0] += 1.4f;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (this.type == ProjectileType.TRIDENT)
                                                                        {
                                                                            if (this.ai[0] == 0f)
                                                                            {
                                                                                this.ai[0] = 4f;
                                                                                this.netUpdate = true;
                                                                            }
                                                                            if (Main.players[this.Owner].itemAnimation < Main.players[this.Owner].inventory[Main.players[this.Owner].selectedItemIndex].UseAnimation / 3)
                                                                            {
                                                                                this.ai[0] -= 1.2f;
                                                                            }
                                                                            else
                                                                            {
                                                                                this.ai[0] += 0.9f;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (this.type == ProjectileType.SPEAR)
                                                                            {
                                                                                if (this.ai[0] == 0f)
                                                                                {
                                                                                    this.ai[0] = 4f;
                                                                                    this.netUpdate = true;
                                                                                }
                                                                                if (Main.players[this.Owner].itemAnimation < Main.players[this.Owner].inventory[Main.players[this.Owner].selectedItemIndex].UseAnimation / 3)
                                                                                {
                                                                                    this.ai[0] -= 1.1f;
                                                                                }
                                                                                else
                                                                                {
                                                                                    this.ai[0] += 0.85f;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    this.Position += this.Velocity * this.ai[0];
                                                                    if (Main.players[this.Owner].itemAnimation == 0)
                                                                    {
                                                                        this.Kill();
                                                                    }
                                                                    this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 2.355f;
                                                                    if (this.type == ProjectileType.LANCE_DARK)
                                                                    {
                                                                        Color newColor;
                                                                        if (Main.rand.Next(5) == 0)
                                                                        {
                                                                            Vector2 arg_3D66_0 = this.Position;
                                                                            int arg_3D66_1 = this.width;
                                                                            int arg_3D66_2 = this.height;
                                                                            int arg_3D66_3 = 14;
                                                                            float arg_3D66_4 = 0f;
                                                                            float arg_3D66_5 = 0f;
                                                                            int arg_3D66_6 = 150;
                                                                            newColor = default(Color);
                                                                            Dust.NewDust(arg_3D66_0, arg_3D66_1, arg_3D66_2, arg_3D66_3, arg_3D66_4, arg_3D66_5, arg_3D66_6, newColor, 1.4f);
                                                                        }
                                                                        Vector2 arg_3DBD_0 = this.Position;
                                                                        int arg_3DBD_1 = this.width;
                                                                        int arg_3DBD_2 = this.height;
                                                                        int arg_3DBD_3 = 27;
                                                                        float arg_3DBD_4 = this.Velocity.X * 0.2f + (float)(this.direction * 3);
                                                                        float arg_3DBD_5 = this.Velocity.Y * 0.2f;
                                                                        int arg_3DBD_6 = 100;
                                                                        newColor = default(Color);
                                                                        int num83 = Dust.NewDust(arg_3DBD_0, arg_3DBD_1, arg_3DBD_2, arg_3DBD_3, arg_3DBD_4, arg_3DBD_5, arg_3DBD_6, newColor, 1.2f);
                                                                        Main.dust[num83].noGravity = true;
                                                                        Dust expr_3DDF_cp_0 = Main.dust[num83];
                                                                        expr_3DDF_cp_0.velocity.X = expr_3DDF_cp_0.velocity.X / 2f;
                                                                        Dust expr_3DFD_cp_0 = Main.dust[num83];
                                                                        expr_3DFD_cp_0.velocity.Y = expr_3DFD_cp_0.velocity.Y / 2f;
                                                                        Vector2 arg_3E55_0 = this.Position - this.Velocity * 2f;
                                                                        int arg_3E55_1 = this.width;
                                                                        int arg_3E55_2 = this.height;
                                                                        int arg_3E55_3 = 27;
                                                                        float arg_3E55_4 = 0f;
                                                                        float arg_3E55_5 = 0f;
                                                                        int arg_3E55_6 = 150;
                                                                        newColor = default(Color);
                                                                        num83 = Dust.NewDust(arg_3E55_0, arg_3E55_1, arg_3E55_2, arg_3E55_3, arg_3E55_4, arg_3E55_5, arg_3E55_6, newColor, 1.4f);
                                                                        Dust expr_3E69_cp_0 = Main.dust[num83];
                                                                        expr_3E69_cp_0.velocity.X = expr_3E69_cp_0.velocity.X / 5f;
                                                                        Dust expr_3E87_cp_0 = Main.dust[num83];
                                                                        expr_3E87_cp_0.velocity.Y = expr_3E87_cp_0.velocity.Y / 5f;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Destroys the projectile
        /// </summary>
        public void Kill()
        {
            if (!this.Active)
            {
                return;
            }
            this.timeLeft = 0;
            switch(this.type)
            {
                case ProjectileType.ARROW_WOODEN:
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 arg_7E_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_7E_1 = this.width;
                            int arg_7E_2 = this.height;
                            int arg_7E_3 = 7;
                            float arg_7E_4 = 0f;
                            float arg_7E_5 = 0f;
                            int arg_7E_6 = 0;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_7E_0, arg_7E_1, arg_7E_2, arg_7E_3, arg_7E_4, arg_7E_5, arg_7E_6, newColor, 1f);
                        }
                    }
                    break;
                case ProjectileType.ARROW_FIRE:
                    {
                        for (int k = 0; k < 20; k++)
                        {
                            Vector2 arg_183_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_183_1 = this.width;
                            int arg_183_2 = this.height;
                            int arg_183_3 = 6;
                            float arg_183_4 = 0f;
                            float arg_183_5 = 0f;
                            int arg_183_6 = 100;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_183_0, arg_183_1, arg_183_2, arg_183_3, arg_183_4, arg_183_5, arg_183_6, newColor, 1f);
                        }
                    }
                    break;
                case ProjectileType.SHURIKEN:
                case ProjectileType.KNIFE_THROWING:
                case ProjectileType.KNIFE_POISONED:
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            Vector2 arg_234_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_234_1 = this.width;
                            int arg_234_2 = this.height;
                            int arg_234_3 = 1;
                            float arg_234_4 = this.Velocity.X * 0.1f;
                            float arg_234_5 = this.Velocity.Y * 0.1f;
                            int arg_234_6 = 0;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_234_0, arg_234_1, arg_234_2, arg_234_3, arg_234_4, arg_234_5, arg_234_6, newColor, 0.75f);
                        }
                    }
                    break;
                case ProjectileType.ARROW_UNHOLY:
                    {
                        for (int m = 0; m < 10; m++)
                        {
                            Vector2 arg_2BF_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_2BF_1 = this.width;
                            int arg_2BF_2 = this.height;
                            int arg_2BF_3 = 14;
                            float arg_2BF_4 = 0f;
                            float arg_2BF_5 = 0f;
                            int arg_2BF_6 = 150;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_2BF_0, arg_2BF_1, arg_2BF_2, arg_2BF_3, arg_2BF_4, arg_2BF_5, arg_2BF_6, newColor, 1.1f);
                        }
                    }
                    break;
                case ProjectileType.ARROW_JESTER:
                    {
                        for (int n = 0; n < 60; n++)
                        {
                            Vector2 arg_351_0 = this.Position;
                            int arg_351_1 = this.width;
                            int arg_351_2 = this.height;
                            int arg_351_3 = 15;
                            float arg_351_4 = this.Velocity.X * 0.5f;
                            float arg_351_5 = this.Velocity.Y * 0.5f;
                            int arg_351_6 = 150;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_351_0, arg_351_1, arg_351_2, arg_351_3, arg_351_4, arg_351_5, arg_351_6, newColor, 1.5f);
                        }
                    }
                    break;
                case ProjectileType.STARFURY:
                    {
                        for (int num = 0; num < 10; num++)
                        {
                            Vector2 position = this.Position;
                            int width = this.width;
                            int height = this.height;
                            int type = 15;
                            float speedX = this.Velocity.X * 0.1f;
                            float speedY = this.Velocity.Y * 0.1f;
                            int alpha = 150;
                            Color newColor = default(Color);
                            Dust.NewDust(position, width, height, type, speedX, speedY, alpha, newColor, 1.2f);
                        }
                        for (int num2 = 0; num2 < 3; num2++)
                        {
                            Gore.NewGore(this.Position, new Vector2(this.Velocity.X * 0.05f, this.Velocity.Y * 0.05f), Main.rand.Next(16, 18));
                        }
                    }
                    break;
                case ProjectileType.FALLEN_STAR:
                    {
                        if (this.damage < 100)
                        {
                            for (int num3 = 0; num3 < 10; num3++)
                            {
                                Vector2 arg_4BA_0 = this.Position;
                                int arg_4BA_1 = this.width;
                                int arg_4BA_2 = this.height;
                                int arg_4BA_3 = 15;
                                float arg_4BA_4 = this.Velocity.X * 0.1f;
                                float arg_4BA_5 = this.Velocity.Y * 0.1f;
                                int arg_4BA_6 = 150;
                                Color newColor = default(Color);
                                Dust.NewDust(arg_4BA_0, arg_4BA_1, arg_4BA_2, arg_4BA_3, arg_4BA_4, arg_4BA_5, arg_4BA_6, newColor, 1.2f);
                            }
                            for (int num4 = 0; num4 < 3; num4++)
                            {
                                Gore.NewGore(this.Position, new Vector2(this.Velocity.X * 0.05f, this.Velocity.Y * 0.05f), Main.rand.Next(16, 18));
                            }
                        }
                    }
                    break;
                case ProjectileType.BALL_MUSKET:
                case ProjectileType.LASER_GREEN:
                case ProjectileType.SHOT_METEOR:
                    {
                        Collision.HitTiles(this.Position, this.Velocity, this.width, this.height);
                    }
                    break;
                case ProjectileType.BALL_OF_FIRE:
                case ProjectileType.FLAMELASH:
                    {
                        for (int num5 = 0; num5 < 20; num5++)
                        {
                            Vector2 arg_61E_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_61E_1 = this.width;
                            int arg_61E_2 = this.height;
                            int arg_61E_3 = 6;
                            float arg_61E_4 = -this.Velocity.X * 0.2f;
                            float arg_61E_5 = -this.Velocity.Y * 0.2f;
                            int arg_61E_6 = 100;
                            Color newColor = default(Color);
                            int num6 = Dust.NewDust(arg_61E_0, arg_61E_1, arg_61E_2, arg_61E_3, arg_61E_4, arg_61E_5, arg_61E_6, newColor, 2f);
                            Main.dust[num6].noGravity = true;
                            Dust expr_63B = Main.dust[num6];
                            expr_63B.velocity *= 2f;
                            Vector2 arg_6AD_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_6AD_1 = this.width;
                            int arg_6AD_2 = this.height;
                            int arg_6AD_3 = 6;
                            float arg_6AD_4 = -this.Velocity.X * 0.2f;
                            float arg_6AD_5 = -this.Velocity.Y * 0.2f;
                            int arg_6AD_6 = 100;
                            newColor = default(Color);
                            num6 = Dust.NewDust(arg_6AD_0, arg_6AD_1, arg_6AD_2, arg_6AD_3, arg_6AD_4, arg_6AD_5, arg_6AD_6, newColor, 1f);
                            Dust expr_6BC = Main.dust[num6];
                            expr_6BC.velocity *= 2f;
                        }
                    }
                    break;
                case ProjectileType.MISSILE_MAGIC:
                    {
                        for (int num7 = 0; num7 < 20; num7++)
                        {
                            Vector2 arg_776_0 = new Vector2(this.Position.X - this.Velocity.X, this.Position.Y - this.Velocity.Y);
                            int arg_776_1 = this.width;
                            int arg_776_2 = this.height;
                            int arg_776_3 = 15;
                            float arg_776_4 = 0f;
                            float arg_776_5 = 0f;
                            int arg_776_6 = 100;
                            Color newColor = default(Color);
                            int num8 = Dust.NewDust(arg_776_0, arg_776_1, arg_776_2, arg_776_3, arg_776_4, arg_776_5, arg_776_6, newColor, 2f);
                            Main.dust[num8].noGravity = true;
                            Dust expr_793 = Main.dust[num8];
                            expr_793.velocity *= 2f;
                            Vector2 arg_804_0 = new Vector2(this.Position.X - this.Velocity.X, this.Position.Y - this.Velocity.Y);
                            int arg_804_1 = this.width;
                            int arg_804_2 = this.height;
                            int arg_804_3 = 15;
                            float arg_804_4 = 0f;
                            float arg_804_5 = 0f;
                            int arg_804_6 = 100;
                            newColor = default(Color);
                            num8 = Dust.NewDust(arg_804_0, arg_804_1, arg_804_2, arg_804_3, arg_804_4, arg_804_5, arg_804_6, newColor, 1f);
                        }
                    }
                    break;
                case ProjectileType.BALL_DIRT:
                    {
                        for (int num9 = 0; num9 < 5; num9++)
                        {
                            Vector2 arg_88F_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_88F_1 = this.width;
                            int arg_88F_2 = this.height;
                            int arg_88F_3 = 0;
                            float arg_88F_4 = 0f;
                            float arg_88F_5 = 0f;
                            int arg_88F_6 = 0;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_88F_0, arg_88F_1, arg_88F_2, arg_88F_3, arg_88F_4, arg_88F_5, arg_88F_6, newColor, 1f);
                        }
                    }
                    break;
                case ProjectileType.BONE:
                    {
                        for (int num16 = 0; num16 < 10; num16++)
                        {
                            Vector2 arg_B18_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_B18_1 = this.width;
                            int arg_B18_2 = this.height;
                            int arg_B18_3 = 26;
                            float arg_B18_4 = 0f;
                            float arg_B18_5 = 0f;
                            int arg_B18_6 = 0;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_B18_0, arg_B18_1, arg_B18_2, arg_B18_3, arg_B18_4, arg_B18_5, arg_B18_6, newColor, 0.8f);
                        }
                    }
                    break;
                case ProjectileType.BALL_SPIKY:
                    {
                        for (int num17 = 0; num17 < 10; num17++)
                        {
                            Vector2 arg_B98_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_B98_1 = this.width;
                            int arg_B98_2 = this.height;
                            int arg_B98_3 = 1;
                            float arg_B98_4 = this.Velocity.X * 0.1f;
                            float arg_B98_5 = this.Velocity.Y * 0.1f;
                            int arg_B98_6 = 0;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_B98_0, arg_B98_1, arg_B98_2, arg_B98_3, arg_B98_4, arg_B98_5, arg_B98_6, newColor, 0.75f);
                        }
                    }
                    break;
                case ProjectileType.BOLT_WATER:
                    {
                        for (int num18 = 0; num18 < 30; num18++)
                        {
                            Vector2 arg_C40_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_C40_1 = this.width;
                            int arg_C40_2 = this.height;
                            int arg_C40_3 = 29;
                            float arg_C40_4 = this.Velocity.X * 0.1f;
                            float arg_C40_5 = this.Velocity.Y * 0.1f;
                            int arg_C40_6 = 100;
                            Color newColor = default(Color);
                            int num19 = Dust.NewDust(arg_C40_0, arg_C40_1, arg_C40_2, arg_C40_3, arg_C40_4, arg_C40_5, arg_C40_6, newColor, 3f);
                            Main.dust[num19].noGravity = true;
                            Vector2 arg_CB1_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_CB1_1 = this.width;
                            int arg_CB1_2 = this.height;
                            int arg_CB1_3 = 29;
                            float arg_CB1_4 = this.Velocity.X * 0.1f;
                            float arg_CB1_5 = this.Velocity.Y * 0.1f;
                            int arg_CB1_6 = 100;
                            newColor = default(Color);
                            Dust.NewDust(arg_CB1_0, arg_CB1_1, arg_CB1_2, arg_CB1_3, arg_CB1_4, arg_CB1_5, arg_CB1_6, newColor, 2f);
                        }
                    }
                    break;
                case ProjectileType.BOMB:
                case ProjectileType.GRENADE:
                case ProjectileType.BOMB_STICKY:
                    {
                        this.Position.X = this.Position.X + (float)(this.width / 2);
                        this.Position.Y = this.Position.Y + (float)(this.height / 2);
                        this.width = 22;
                        this.height = 22;
                        this.Position.X = this.Position.X - (float)(this.width / 2);
                        this.Position.Y = this.Position.Y - (float)(this.height / 2);
                        for (int num27 = 0; num27 < 20; num27++)
                        {
                            Vector2 arg_12DE_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_12DE_1 = this.width;
                            int arg_12DE_2 = this.height;
                            int arg_12DE_3 = 31;
                            float arg_12DE_4 = 0f;
                            float arg_12DE_5 = 0f;
                            int arg_12DE_6 = 100;
                            Color newColor = default(Color);
                            int num28 = Dust.NewDust(arg_12DE_0, arg_12DE_1, arg_12DE_2, arg_12DE_3, arg_12DE_4, arg_12DE_5, arg_12DE_6, newColor, 1.5f);
                            Dust expr_12ED = Main.dust[num28];
                            expr_12ED.velocity *= 1.4f;
                        }
                        for (int num29 = 0; num29 < 10; num29++)
                        {
                            Vector2 arg_1359_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_1359_1 = this.width;
                            int arg_1359_2 = this.height;
                            int arg_1359_3 = 6;
                            float arg_1359_4 = 0f;
                            float arg_1359_5 = 0f;
                            int arg_1359_6 = 100;
                            Color newColor = default(Color);
                            int num30 = Dust.NewDust(arg_1359_0, arg_1359_1, arg_1359_2, arg_1359_3, arg_1359_4, arg_1359_5, arg_1359_6, newColor, 2.5f);
                            Main.dust[num30].noGravity = true;
                            Dust expr_1376 = Main.dust[num30];
                            expr_1376.velocity *= 5f;
                            Vector2 arg_13CE_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_13CE_1 = this.width;
                            int arg_13CE_2 = this.height;
                            int arg_13CE_3 = 6;
                            float arg_13CE_4 = 0f;
                            float arg_13CE_5 = 0f;
                            int arg_13CE_6 = 100;
                            newColor = default(Color);
                            num30 = Dust.NewDust(arg_13CE_0, arg_13CE_1, arg_13CE_2, arg_13CE_3, arg_13CE_4, arg_13CE_5, arg_13CE_6, newColor, 1.5f);
                            Dust expr_13DD = Main.dust[num30];
                            expr_13DD.velocity *= 3f;
                        }
                        Vector2 arg_1434_0 = new Vector2(this.Position.X, this.Position.Y);
                        Vector2 vector = default(Vector2);
                        int num31 = Gore.NewGore(arg_1434_0, vector, Main.rand.Next(61, 64));
                        Gore expr_1443 = Main.gore[num31];
                        expr_1443.velocity *= 0.4f;
                        Gore expr_1465_cp_0 = Main.gore[num31];
                        expr_1465_cp_0.velocity.X = expr_1465_cp_0.velocity.X + 1f;
                        Gore expr_1483_cp_0 = Main.gore[num31];
                        expr_1483_cp_0.velocity.Y = expr_1483_cp_0.velocity.Y + 1f;
                        Vector2 arg_14C7_0 = new Vector2(this.Position.X, this.Position.Y);
                        vector = default(Vector2);
                        num31 = Gore.NewGore(arg_14C7_0, vector, Main.rand.Next(61, 64));
                        Gore expr_14D6 = Main.gore[num31];
                        expr_14D6.velocity *= 0.4f;
                        Gore expr_14F8_cp_0 = Main.gore[num31];
                        expr_14F8_cp_0.velocity.X = expr_14F8_cp_0.velocity.X - 1f;
                        Gore expr_1516_cp_0 = Main.gore[num31];
                        expr_1516_cp_0.velocity.Y = expr_1516_cp_0.velocity.Y + 1f;
                        Vector2 arg_155A_0 = new Vector2(this.Position.X, this.Position.Y);
                        vector = default(Vector2);
                        num31 = Gore.NewGore(arg_155A_0, vector, Main.rand.Next(61, 64));
                        Gore expr_1569 = Main.gore[num31];
                        expr_1569.velocity *= 0.4f;
                        Gore expr_158B_cp_0 = Main.gore[num31];
                        expr_158B_cp_0.velocity.X = expr_158B_cp_0.velocity.X + 1f;
                        Gore expr_15A9_cp_0 = Main.gore[num31];
                        expr_15A9_cp_0.velocity.Y = expr_15A9_cp_0.velocity.Y - 1f;
                        Vector2 arg_15ED_0 = new Vector2(this.Position.X, this.Position.Y);
                        vector = default(Vector2);
                        num31 = Gore.NewGore(arg_15ED_0, vector, Main.rand.Next(61, 64));
                        Gore expr_15FC = Main.gore[num31];
                        expr_15FC.velocity *= 0.4f;
                        Gore expr_161E_cp_0 = Main.gore[num31];
                        expr_161E_cp_0.velocity.X = expr_161E_cp_0.velocity.X - 1f;
                        Gore expr_163C_cp_0 = Main.gore[num31];
                        expr_163C_cp_0.velocity.Y = expr_163C_cp_0.velocity.Y - 1f;
                    }
                    break;
                case ProjectileType.DYNAMITE:
                    {
                        this.Position.X = this.Position.X + (float)(this.width / 2);
                        this.Position.Y = this.Position.Y + (float)(this.height / 2);
                        this.width = 200;
                        this.height = 200;
                        this.Position.X = this.Position.X - (float)(this.width / 2);
                        this.Position.Y = this.Position.Y - (float)(this.height / 2);
                        for (int num32 = 0; num32 < 50; num32++)
                        {
                            Vector2 arg_174A_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_174A_1 = this.width;
                            int arg_174A_2 = this.height;
                            int arg_174A_3 = 31;
                            float arg_174A_4 = 0f;
                            float arg_174A_5 = 0f;
                            int arg_174A_6 = 100;
                            Color newColor = default(Color);
                            int num33 = Dust.NewDust(arg_174A_0, arg_174A_1, arg_174A_2, arg_174A_3, arg_174A_4, arg_174A_5, arg_174A_6, newColor, 2f);
                            Dust expr_1759 = Main.dust[num33];
                            expr_1759.velocity *= 1.4f;
                        }
                        for (int num34 = 0; num34 < 80; num34++)
                        {
                            Vector2 arg_17C5_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_17C5_1 = this.width;
                            int arg_17C5_2 = this.height;
                            int arg_17C5_3 = 6;
                            float arg_17C5_4 = 0f;
                            float arg_17C5_5 = 0f;
                            int arg_17C5_6 = 100;
                            Color newColor = default(Color);
                            int num35 = Dust.NewDust(arg_17C5_0, arg_17C5_1, arg_17C5_2, arg_17C5_3, arg_17C5_4, arg_17C5_5, arg_17C5_6, newColor, 3f);
                            Main.dust[num35].noGravity = true;
                            Dust expr_17E2 = Main.dust[num35];
                            expr_17E2.velocity *= 5f;
                            Vector2 arg_183A_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_183A_1 = this.width;
                            int arg_183A_2 = this.height;
                            int arg_183A_3 = 6;
                            float arg_183A_4 = 0f;
                            float arg_183A_5 = 0f;
                            int arg_183A_6 = 100;
                            newColor = default(Color);
                            num35 = Dust.NewDust(arg_183A_0, arg_183A_1, arg_183A_2, arg_183A_3, arg_183A_4, arg_183A_5, arg_183A_6, newColor, 2f);
                            Dust expr_1849 = Main.dust[num35];
                            expr_1849.velocity *= 3f;
                        }
                        for (int num36 = 0; num36 < 2; num36++)
                        {
                            Vector2 arg_18C8_0 = new Vector2(this.Position.X + (float)(this.width / 2) - 24f, this.Position.Y + (float)(this.height / 2) - 24f);
                            Vector2 vector = default(Vector2);
                            int num37 = Gore.NewGore(arg_18C8_0, vector, Main.rand.Next(61, 64));
                            Main.gore[num37].scale = 1.5f;
                            Gore expr_18EE_cp_0 = Main.gore[num37];
                            expr_18EE_cp_0.velocity.X = expr_18EE_cp_0.velocity.X + 1.5f;
                            Gore expr_190C_cp_0 = Main.gore[num37];
                            expr_190C_cp_0.velocity.Y = expr_190C_cp_0.velocity.Y + 1.5f;
                            Vector2 arg_1970_0 = new Vector2(this.Position.X + (float)(this.width / 2) - 24f, this.Position.Y + (float)(this.height / 2) - 24f);
                            vector = default(Vector2);
                            num37 = Gore.NewGore(arg_1970_0, vector, Main.rand.Next(61, 64));
                            Main.gore[num37].scale = 1.5f;
                            Gore expr_1996_cp_0 = Main.gore[num37];
                            expr_1996_cp_0.velocity.X = expr_1996_cp_0.velocity.X - 1.5f;
                            Gore expr_19B4_cp_0 = Main.gore[num37];
                            expr_19B4_cp_0.velocity.Y = expr_19B4_cp_0.velocity.Y + 1.5f;
                            Vector2 arg_1A18_0 = new Vector2(this.Position.X + (float)(this.width / 2) - 24f, this.Position.Y + (float)(this.height / 2) - 24f);
                            vector = default(Vector2);
                            num37 = Gore.NewGore(arg_1A18_0, vector, Main.rand.Next(61, 64));
                            Main.gore[num37].scale = 1.5f;
                            Gore expr_1A3E_cp_0 = Main.gore[num37];
                            expr_1A3E_cp_0.velocity.X = expr_1A3E_cp_0.velocity.X + 1.5f;
                            Gore expr_1A5C_cp_0 = Main.gore[num37];
                            expr_1A5C_cp_0.velocity.Y = expr_1A5C_cp_0.velocity.Y - 1.5f;
                            Vector2 arg_1AC0_0 = new Vector2(this.Position.X + (float)(this.width / 2) - 24f, this.Position.Y + (float)(this.height / 2) - 24f);
                            vector = default(Vector2);
                            num37 = Gore.NewGore(arg_1AC0_0, vector, Main.rand.Next(61, 64));
                            Main.gore[num37].scale = 1.5f;
                            Gore expr_1AE6_cp_0 = Main.gore[num37];
                            expr_1AE6_cp_0.velocity.X = expr_1AE6_cp_0.velocity.X - 1.5f;
                            Gore expr_1B04_cp_0 = Main.gore[num37];
                            expr_1B04_cp_0.velocity.Y = expr_1B04_cp_0.velocity.Y - 1.5f;
                        }
                        this.Position.X = this.Position.X + (float)(this.width / 2);
                        this.Position.Y = this.Position.Y + (float)(this.height / 2);
                        this.width = 10;
                        this.height = 10;
                        this.Position.X = this.Position.X - (float)(this.width / 2);
                        this.Position.Y = this.Position.Y - (float)(this.height / 2);
                    }
                    break;
                case ProjectileType.BALL_SAND_DROP:
                case ProjectileType.BALL_SAND_GUN:
                    {
                        for (int num10 = 0; num10 < 5; num10++)
                        {
                            Vector2 arg_923_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_923_1 = this.width;
                            int arg_923_2 = this.height;
                            int arg_923_3 = 32;
                            float arg_923_4 = 0f;
                            float arg_923_5 = 0f;
                            int arg_923_6 = 0;
                            Color newColor = default(Color);
                            int num11 = Dust.NewDust(arg_923_0, arg_923_1, arg_923_2, arg_923_3, arg_923_4, arg_923_5, arg_923_6, newColor, 1f);
                            Dust expr_932 = Main.dust[num11];
                            expr_932.velocity *= 0.6f;
                        }
                    }
                    break;
                case ProjectileType.FEATHER_HARPY:
                    {
                        for (int num20 = 0; num20 < 10; num20++)
                        {
                            Vector2 arg_D35_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_D35_1 = this.width;
                            int arg_D35_2 = this.height;
                            int arg_D35_3 = 42;
                            float arg_D35_4 = this.Velocity.X * 0.1f;
                            float arg_D35_5 = this.Velocity.Y * 0.1f;
                            int arg_D35_6 = 0;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_D35_0, arg_D35_1, arg_D35_2, arg_D35_3, arg_D35_4, arg_D35_5, arg_D35_6, newColor, 1f);
                        }
                    }
                    break;
                case ProjectileType.BALL_MUD:
                    {
                        for (int num12 = 0; num12 < 5; num12++)
                        {
                            Vector2 arg_9CB_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_9CB_1 = this.width;
                            int arg_9CB_2 = this.height;
                            int arg_9CB_3 = 38;
                            float arg_9CB_4 = 0f;
                            float arg_9CB_5 = 0f;
                            int arg_9CB_6 = 0;
                            Color newColor = default(Color);
                            int num13 = Dust.NewDust(arg_9CB_0, arg_9CB_1, arg_9CB_2, arg_9CB_3, arg_9CB_4, arg_9CB_5, arg_9CB_6, newColor, 1f);
                            Dust expr_9DA = Main.dust[num13];
                            expr_9DA.velocity *= 0.6f;
                        }
                    }
                    break;
                case ProjectileType.BALL_ASH:
                    {
                        for (int num14 = 0; num14 < 5; num14++)
                        {
                            Vector2 arg_A73_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_A73_1 = this.width;
                            int arg_A73_2 = this.height;
                            int arg_A73_3 = 36;
                            float arg_A73_4 = 0f;
                            float arg_A73_5 = 0f;
                            int arg_A73_6 = 0;
                            Color newColor = default(Color);
                            int num15 = Dust.NewDust(arg_A73_0, arg_A73_1, arg_A73_2, arg_A73_3, arg_A73_4, arg_A73_5, arg_A73_6, newColor, 1f);
                            Dust expr_A82 = Main.dust[num15];
                            expr_A82.velocity *= 0.6f;
                        }
                    }
                    break;
                case ProjectileType.ARROW_HELLFIRE:
                    {
                        for (int num23 = 0; num23 < 10; num23++)
                        {
                            Vector2 arg_ED0_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_ED0_1 = this.width;
                            int arg_ED0_2 = this.height;
                            int arg_ED0_3 = 31;
                            float arg_ED0_4 = 0f;
                            float arg_ED0_5 = 0f;
                            int arg_ED0_6 = 100;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_ED0_0, arg_ED0_1, arg_ED0_2, arg_ED0_3, arg_ED0_4, arg_ED0_5, arg_ED0_6, newColor, 1.5f);
                        }
                        for (int num24 = 0; num24 < 5; num24++)
                        {
                            Vector2 arg_F2D_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_F2D_1 = this.width;
                            int arg_F2D_2 = this.height;
                            int arg_F2D_3 = 6;
                            float arg_F2D_4 = 0f;
                            float arg_F2D_5 = 0f;
                            int arg_F2D_6 = 100;
                            Color newColor = default(Color);
                            int num25 = Dust.NewDust(arg_F2D_0, arg_F2D_1, arg_F2D_2, arg_F2D_3, arg_F2D_4, arg_F2D_5, arg_F2D_6, newColor, 2.5f);
                            Main.dust[num25].noGravity = true;
                            Dust expr_F4A = Main.dust[num25];
                            expr_F4A.velocity *= 3f;
                            Vector2 arg_FA2_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_FA2_1 = this.width;
                            int arg_FA2_2 = this.height;
                            int arg_FA2_3 = 6;
                            float arg_FA2_4 = 0f;
                            float arg_FA2_5 = 0f;
                            int arg_FA2_6 = 100;
                            newColor = default(Color);
                            num25 = Dust.NewDust(arg_FA2_0, arg_FA2_1, arg_FA2_2, arg_FA2_3, arg_FA2_4, arg_FA2_5, arg_FA2_6, newColor, 1.5f);
                            Dust expr_FB1 = Main.dust[num25];
                            expr_FB1.velocity *= 2f;
                        }
                        Vector2 arg_1007_0 = new Vector2(this.Position.X, this.Position.Y);
                        Vector2 vector = default(Vector2);
                        int num26 = Gore.NewGore(arg_1007_0, vector, Main.rand.Next(61, 64));
                        Gore expr_1016 = Main.gore[num26];
                        expr_1016.velocity *= 0.4f;
                        Gore expr_1038_cp_0 = Main.gore[num26];
                        expr_1038_cp_0.velocity.X = expr_1038_cp_0.velocity.X + (float)Main.rand.Next(-10, 11) * 0.1f;
                        Gore expr_1066_cp_0 = Main.gore[num26];
                        expr_1066_cp_0.velocity.Y = expr_1066_cp_0.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.1f;
                        Vector2 arg_10BA_0 = new Vector2(this.Position.X, this.Position.Y);
                        vector = default(Vector2);
                        num26 = Gore.NewGore(arg_10BA_0, vector, Main.rand.Next(61, 64));
                        Gore expr_10C9 = Main.gore[num26];
                        expr_10C9.velocity *= 0.4f;
                        Gore expr_10EB_cp_0 = Main.gore[num26];
                        expr_10EB_cp_0.velocity.X = expr_10EB_cp_0.velocity.X + (float)Main.rand.Next(-10, 11) * 0.1f;
                        Gore expr_1119_cp_0 = Main.gore[num26];
                        expr_1119_cp_0.velocity.Y = expr_1119_cp_0.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.1f;
                        if (this.Owner == Main.myPlayer)
                        {
                            this.penetrate = -1;
                            this.Position.X = this.Position.X + (float)(this.width / 2);
                            this.Position.Y = this.Position.Y + (float)(this.height / 2);
                            this.width = 64;
                            this.height = 64;
                            this.Position.X = this.Position.X - (float)(this.width / 2);
                            this.Position.Y = this.Position.Y - (float)(this.height / 2);
                            this.Damage();
                        }
                    }
                    break;
                case ProjectileType.SICKLE_DEMON:
                case ProjectileType.SCYTHE_DEMON:
                    {
                        for (int num21 = 0; num21 < 30; num21++)
                        {
                            Vector2 arg_DDB_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_DDB_1 = this.width;
                            int arg_DDB_2 = this.height;
                            int arg_DDB_3 = 27;
                            float arg_DDB_4 = this.Velocity.X;
                            float arg_DDB_5 = this.Velocity.Y;
                            int arg_DDB_6 = 100;
                            Color newColor = default(Color);
                            int num22 = Dust.NewDust(arg_DDB_0, arg_DDB_1, arg_DDB_2, arg_DDB_3, arg_DDB_4, arg_DDB_5, arg_DDB_6, newColor, 1.7f);
                            Main.dust[num22].noGravity = true;
                            Vector2 arg_E40_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_E40_1 = this.width;
                            int arg_E40_2 = this.height;
                            int arg_E40_3 = 27;
                            float arg_E40_4 = this.Velocity.X;
                            float arg_E40_5 = this.Velocity.Y;
                            int arg_E40_6 = 100;
                            newColor = default(Color);
                            Dust.NewDust(arg_E40_0, arg_E40_1, arg_E40_2, arg_E40_3, arg_E40_4, arg_E40_5, arg_E40_6, newColor, 1f);
                        }
                    }
                    break;
                case ProjectileType.SEED:
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            Vector2 arg_101_0 = new Vector2(this.Position.X, this.Position.Y);
                            int arg_101_1 = this.width;
                            int arg_101_2 = this.height;
                            int arg_101_3 = 0;
                            float arg_101_4 = 0f;
                            float arg_101_5 = 0f;
                            int arg_101_6 = 0;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_101_0, arg_101_1, arg_101_2, arg_101_3, arg_101_4, arg_101_5, arg_101_6, newColor, 0.7f);
                        }
                    }
                    break;
                default:
                    break;
            }
            if (this.Owner == Main.myPlayer)
            {
                if (this.type == ProjectileType.BOMB || this.type == ProjectileType.DYNAMITE || this.type == ProjectileType.BOMB_STICKY)
                {
                    int num38 = 3;
                    if (this.type == ProjectileType.DYNAMITE)
                    {
                        num38 = 7;
                    }
                    int num39 = (int)(this.Position.X / 16f - (float)num38);
                    int num40 = (int)(this.Position.X / 16f + (float)num38);
                    int num41 = (int)(this.Position.Y / 16f - (float)num38);
                    int num42 = (int)(this.Position.Y / 16f + (float)num38);
                    if (num39 < 0)
                    {
                        num39 = 0;
                    }
                    if (num40 > Main.maxTilesX)
                    {
                        num40 = Main.maxTilesX;
                    }
                    if (num41 < 0)
                    {
                        num41 = 0;
                    }
                    if (num42 > Main.maxTilesY)
                    {
                        num42 = Main.maxTilesY;
                    }
                    bool flag = false;
                    for (int num43 = num39; num43 <= num40; num43++)
                    {
                        for (int num44 = num41; num44 <= num42; num44++)
                        {
                            float num45 = Math.Abs((float)num43 - this.Position.X / 16f);
                            float num46 = Math.Abs((float)num44 - this.Position.Y / 16f);
                            double num47 = Math.Sqrt((double)(num45 * num45 + num46 * num46));
                            if (num47 < (double)num38 && Main.tile[num43, num44] != null && Main.tile[num43, num44].wall == 0)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    for (int num48 = num39; num48 <= num40; num48++)
                    {
                        for (int num49 = num41; num49 <= num42; num49++)
                        {
                            float num50 = Math.Abs((float)num48 - this.Position.X / 16f);
                            float num51 = Math.Abs((float)num49 - this.Position.Y / 16f);
                            double num52 = Math.Sqrt((double)(num50 * num50 + num51 * num51));
                            if (num52 < (double)num38)
                            {
                                bool flag2 = true;
                                if (Main.tile[num48, num49] != null && Main.tile[num48, num49].Active)
                                {
                                    flag2 = false;
                                    if (this.type == ProjectileType.BOMB || this.type == ProjectileType.BOMB_STICKY)
                                    {
                                        if (!Main.tileSolid[(int)Main.tile[num48, num49].type] || Main.tileSolidTop[(int)Main.tile[num48, num49].type] || Main.tile[num48, num49].type == 0 || Main.tile[num48, num49].type == 1 || Main.tile[num48, num49].type == 2 || Main.tile[num48, num49].type == 23 || Main.tile[num48, num49].type == 30 || Main.tile[num48, num49].type == 40 || Main.tile[num48, num49].type == 6 || Main.tile[num48, num49].type == 7 || Main.tile[num48, num49].type == 8 || Main.tile[num48, num49].type == 9 || Main.tile[num48, num49].type == 10 || Main.tile[num48, num49].type == 53 || Main.tile[num48, num49].type == 54 || Main.tile[num48, num49].type == 57 || Main.tile[num48, num49].type == 59 || Main.tile[num48, num49].type == 60 || Main.tile[num48, num49].type == 63 || Main.tile[num48, num49].type == 64 || Main.tile[num48, num49].type == 65 || Main.tile[num48, num49].type == 66 || Main.tile[num48, num49].type == 67 || Main.tile[num48, num49].type == 68 || Main.tile[num48, num49].type == 70 || Main.tile[num48, num49].type == 37)
                                        {
                                            flag2 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (this.type == ProjectileType.DYNAMITE)
                                        {
                                            flag2 = true;
                                        }
                                    }
                                    if (Main.tileDungeon[(int)Main.tile[num48, num49].type] || Main.tile[num48, num49].type == 26 || Main.tile[num48, num49].type == 58 || Main.tile[num48, num49].type == 21)
                                    {
                                        flag2 = false;
                                    }
                                    if (flag2)
                                    {
                                        WorldGen.KillTile(num48, num49, false, false, false);
                                    }
                                }
                                if (flag2 && Main.tile[num48, num49] != null && Main.tile[num48, num49].wall > 0 && flag)
                                {
                                    WorldGen.KillWall(num48, num49, false);
                                }
                            }
                        }
                    }
                }
                
                NetMessage.SendData(29, -1, -1, "", this.identity, (float)this.Owner);

                int num53 = -1;
                if (this.aiStyle == 10)
                {
                    int num54 = (int)(this.Position.X + (float)(this.width / 2)) / 16;
                    int num55 = (int)(this.Position.Y + (float)(this.width / 2)) / 16;
                    int num56 = 0;
                    int num57 = 2;
                    if (this.type == ProjectileType.BALL_SAND_DROP)
                    {
                        num56 = 53;
                        num57 = 0;
                    }
                    if (this.type == ProjectileType.BALL_SAND_GUN)
                    {
                        num56 = 53;
                        num57 = 0;
                    }
                    else
                    {
                        if (this.type == ProjectileType.BALL_MUD)
                        {
                            num56 = 59;
                            num57 = 176;
                        }
                        else
                        {
                            if (this.type == ProjectileType.BALL_ASH)
                            {
                                num56 = 57;
                                num57 = 172;
                            }
                        }
                    }
                    if (!Main.tile[num54, num55].Active)
                    {
                        WorldGen.PlaceTile(num54, num55, num56, false, true, -1, 0);
                        if (Main.tile[num54, num55].Active && (int)Main.tile[num54, num55].type == num56)
                        {
                            NetMessage.SendData(17, -1, -1, "", 1, (float)num54, (float)num55, (float)num56);
                        }
                        else
                        {
                            if (num57 > 0)
                            {
                                num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, num57, 1, false);
                            }
                        }
                    }
                    else
                    {
                        if (num57 > 0)
                        {
                            num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, num57, 1, false);
                        }
                    }
                }
                if (this.type == ProjectileType.ARROW_WOODEN && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 40, 1, false);
                }
                if (this.type == ProjectileType.ARROW_FIRE && Main.rand.Next(2) == 0)
                {
                    if (Main.rand.Next(3) == 0)
                    {
                        num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 41, 1, false);
                    }
                    else
                    {
                        num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 40, 1, false);
                    }
                }
                if (this.type == ProjectileType.GLOWSTICK && Main.rand.Next(3) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 282, 1, false);
                }
                if (this.type == ProjectileType.GLOWSTICK_STICKY && Main.rand.Next(3) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 286, 1, false);
                }
                if (this.type == ProjectileType.KNIFE_THROWING && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 279, 1, false);
                }
                if (this.type == ProjectileType.KNIFE_POISONED && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 287, 1, false);
                }
                if (this.type == ProjectileType.SHURIKEN && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 42, 1, false);
                }
                if (this.type == ProjectileType.ARROW_UNHOLY && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 47, 1, false);
                }
                if (this.type == ProjectileType.FALLEN_STAR && this.damage > 100)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 75, 1, false);
                }
                if (this.type == ProjectileType.BONE && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 154, 1, false);
                }
            }
            this.Active = false;
        }

        /// <summary>
        /// Creates a new color by combining newColor with the projectiles alpha value
        /// </summary>
        /// <param name="newColor">New color to combine</param>
        /// <returns>Combined color</returns>
        public Color GetAlpha(Color newColor)
        {
            int r;
            int g;
            int b;
            if (this.type == ProjectileType.STARFURY || this.type == ProjectileType.BALL_OF_FIRE || this.type == ProjectileType.FLAMELASH || this.type == ProjectileType.GLOWSTICK || this.type == ProjectileType.GLOWSTICK_STICKY)
            {
                r = (int)newColor.R - this.alpha / 3;
                g = (int)newColor.G - this.alpha / 3;
                b = (int)newColor.B - this.alpha / 3;
            }
            else
            {
                if (this.type == ProjectileType.MISSILE_MAGIC || this.type == ProjectileType.ORB_OF_LIGHT || this.type == ProjectileType.SICKLE_DEMON || this.type == ProjectileType.SCYTHE_DEMON)
                {
                    r = (int)newColor.R;
                    g = (int)newColor.G;
                    b = (int)newColor.B;
                }
                else
                {
                    r = (int)newColor.R - this.alpha;
                    g = (int)newColor.G - this.alpha;
                    b = (int)newColor.B - this.alpha;
                }
            }
            int num = (int)newColor.A - this.alpha;
            if (num < 0)
            {
                num = 0;
            }
            if (num > 255)
            {
                num = 255;
            }
            return new Color(r, g, b, num);
        }
    }
}
