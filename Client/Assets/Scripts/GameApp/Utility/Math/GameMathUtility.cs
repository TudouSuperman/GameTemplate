using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 游戏数学工具类
    /// </summary>
    public static class GameMathUtility
    {
        /// <summary>
        /// 计算伤害衰减（基于距离）
        /// </summary>
        public static float DamageFalloff(float maxDamage, float minDamage, float maxRange, float distance)
        {
            distance = Mathf.Clamp(distance, 0, maxRange);
            float t = distance / maxRange;
            return Mathf.Lerp(maxDamage, minDamage, t * t); // 平方衰减更真实
        }

        /// <summary>
        /// 计算经验值需求（指数增长）
        /// </summary>
        public static int ExperienceForLevel(int level, int baseExp, float growthFactor) => Mathf.FloorToInt(baseExp * Mathf.Pow(growthFactor, level - 1));

        /// <summary>
        /// 计算抛射物初始速度（忽略空气阻力）
        /// </summary>
        public static Vector3 CalculateProjectileVelocity(Vector3 start, Vector3 target, float gravity, float angle)
        {
            Vector3 direction = target - start;
            float height = direction.y;
            direction.y = 0;
            float horizontalDistance = direction.magnitude;

            float radianAngle = angle * Mathf.Deg2Rad;
            float velocity = Mathf.Sqrt(horizontalDistance * gravity / Mathf.Sin(2 * radianAngle));

            Vector3 launchDirection = direction.normalized;
            launchDirection.y = Mathf.Tan(radianAngle);

            return launchDirection.normalized * velocity;
        }

        /// <summary>
        /// 计算碰撞后的反弹向量
        /// </summary>
        public static Vector3 CalculateBounceDirection(Vector3 velocity, Vector3 normal, float elasticity)
        {
            float velocityDotNormal = Vector3.Dot(velocity, normal);
            if (velocityDotNormal > 0) return velocity;

            Vector3 bounce = velocity - (1 + elasticity) * velocityDotNormal * normal;
            return bounce;
        }

        /// <summary>
        /// 计算击退力（基于质量和距离）
        /// </summary>
        public static Vector3 CalculateKnockbackForce(Vector3 attackerPos, Vector3 targetPos, float baseForce, float attackerMass = 1f, float targetMass = 1f)
        {
            Vector3 direction = (targetPos - attackerPos).normalized;
            float distance = Vector3.Distance(attackerPos, targetPos);
            float forceMultiplier = baseForce * (attackerMass / targetMass) / (distance + 1f);

            return direction * forceMultiplier;
        }

        /// <summary>
        /// 计算技能冷却时间（考虑冷却缩减）
        /// </summary>
        public static float CalculateCooldown(float baseCooldown, float cooldownReduction)
        {
            cooldownReduction = Mathf.Clamp(cooldownReduction, 0f, 0.9f); // 通常冷却缩减有上限
            return baseCooldown * (1f - cooldownReduction);
        }

        /// <summary>
        /// 计算暴击伤害
        /// </summary>
        public static float CalculateCritDamage(float baseDamage, float critChance, float critMultiplier)
        {
            bool isCrit = RandomUtility.Random100(critChance);
            return isCrit ? baseDamage * critMultiplier : baseDamage;
        }

        /// <summary>
        /// 计算护甲减伤
        /// </summary>
        public static float CalculateArmorReduction(float damage, float armor)
        {
            // 常见的护甲减伤公式：伤害减少 = 护甲 / (护甲 + 常数)
            float reduction = armor / (armor + 100f); // 常数可以根据游戏平衡调整
            return damage * (1f - reduction);
        }

        /// <summary>
        /// 计算元素抗性减伤
        /// </summary>
        public static float CalculateElementalReduction(float damage, float resistance)
        {
            // 元素抗性通常直接按百分比减少伤害
            resistance = Mathf.Clamp(resistance, -100f, 100f); // 限制在-100%到100%之间
            return damage * (1f - resistance / 100f);
        }

        /// <summary>
        /// 计算等级差异修正（用于平衡不同等级玩家/怪物之间的互动）
        /// </summary>
        public static float CalculateLevelDifferenceModifier(int attackerLevel, int targetLevel)
        {
            int levelDiff = targetLevel - attackerLevel;

            // 等级比目标高，获得增益
            if (levelDiff < 0)
            {
                return 1f + Mathf.Abs(levelDiff) * 0.05f; // 每高一级增加5%效果
            }
            // 等级比目标低，受到惩罚
            else if (levelDiff > 0)
            {
                return 1f / (1f + levelDiff * 0.05f); // 每低一级减少5%效果
            }

            return 1f; // 同级无修正
        }

        /// <summary>
        /// 计算连击加成（基于连击数）
        /// </summary>
        public static float CalculateComboBonus(float baseDamage, int comboCount, float comboMultiplier = 0.1f)
        {
            return baseDamage * (1f + comboCount * comboMultiplier);
        }

        /// <summary>
        /// 计算属性点收益（递减收益模型）
        /// </summary>
        public static float CalculateAttributeBonus(float baseValue, int attributePoints, float diminishingFactor = 0.9f)
        {
            float totalBonus = 0f;
            float currentBonus = 1f;

            for (int i = 0; i < attributePoints; i++)
            {
                totalBonus += currentBonus;
                currentBonus *= diminishingFactor; // 每点收益递减
            }

            return baseValue * (1f + totalBonus / 10f); // 除以10用于平衡数值
        }

        /// <summary>
        /// 计算战斗评分（用于匹配系统）
        /// </summary>
        public static int CalculateCombatRating(int level, float averageItemLevel, int wins, int losses)
        {
            float winRate = losses > 0 ? (float)wins / (wins + losses) : 1f;
            return Mathf.RoundToInt(level * 10 + averageItemLevel * 5 + winRate * 200);
        }

        /// <summary>
        /// 计算升级所需时间（基于当前等级）
        /// </summary>
        public static float CalculateLevelUpTime(int currentLevel, float baseTime, float timeMultiplier = 1.15f)
        {
            return baseTime * Mathf.Pow(timeMultiplier, currentLevel - 1);
        }

        /// <summary>
        /// 计算技能范围（基于技能等级）
        /// </summary>
        public static float CalculateSkillRange(int skillLevel, float baseRange, float rangePerLevel = 0.5f)
        {
            return baseRange + (skillLevel - 1) * rangePerLevel;
        }

        /// <summary>
        /// 计算技能持续时间（基于技能等级）
        /// </summary>
        public static float CalculateSkillDuration(int skillLevel, float baseDuration, float durationPerLevel = 0.2f)
        {
            return baseDuration + (skillLevel - 1) * durationPerLevel;
        }

        /// <summary>
        /// 计算治疗效果（基于法强和治疗系数）
        /// </summary>
        public static float CalculateHealingAmount(float spellPower, float healingCoefficient, float baseHealing = 0f)
        {
            return baseHealing + spellPower * healingCoefficient;
        }

        /// <summary>
        /// 计算吸血效果（基于造成伤害和吸血比例）
        /// </summary>
        public static float CalculateLifeSteal(float damageDealt, float lifeStealPercentage)
        {
            return damageDealt * lifeStealPercentage;
        }

        /// <summary>
        /// 计算击杀奖励（基于目标价值和等级差异）
        /// </summary>
        public static int CalculateKillReward(int targetValue, int killerLevel, int targetLevel)
        {
            float levelModifier = CalculateLevelDifferenceModifier(killerLevel, targetLevel);
            return Mathf.RoundToInt(targetValue * levelModifier);
        }

        /// <summary>
        /// 计算移动速度加成（基于敏捷或其他属性）
        /// </summary>
        public static float CalculateMovementSpeedBonus(float baseSpeed, int agility, float agilityFactor = 0.01f)
        {
            return baseSpeed * (1f + agility * agilityFactor);
        }

        /// <summary>
        /// 计算攻击速度加成（基于敏捷或其他属性）
        /// </summary>
        public static float CalculateAttackSpeedBonus(float baseSpeed, int agility, float agilityFactor = 0.01f)
        {
            return baseSpeed / (1f + agility * agilityFactor); // 攻击速度通常表示为攻击间隔
        }

        /// <summary>
        /// 计算闪避概率（基于敏捷和等级）
        /// </summary>
        public static float CalculateDodgeChance(int agility, int level, float baseChance = 0.01f)
        {
            return Mathf.Clamp(baseChance + agility * 0.001f + level * 0.0005f, 0f, 0.95f); // 闪避概率通常有上限
        }

        /// <summary>
        /// 计算暴击概率（基于幸运和等级）
        /// </summary>
        public static float CalculateCritChance(int luck, int level, float baseChance = 0.05f)
        {
            return Mathf.Clamp(baseChance + luck * 0.002f + level * 0.001f, 0f, 1f);
        }

        /// <summary>
        /// 计算格挡减伤（基于格挡几率和格挡值）
        /// </summary>
        public static float CalculateBlockReduction(float incomingDamage, float blockChance, float blockValue)
        {
            bool isBlocked = RandomUtility.Random100(blockChance * 100f);
            return isBlocked ? Mathf.Max(0, incomingDamage - blockValue) : incomingDamage;
        }

        /// <summary>
        /// 计算技能消耗（基于技能等级和智力）
        /// </summary>
        public static float CalculateSkillCost(int skillLevel, int intelligence, float baseCost, float reductionPerLevel = 0.05f)
        {
            float levelReduction = (skillLevel - 1) * reductionPerLevel;
            float intReduction = intelligence * 0.01f;
            return baseCost * (1f - Mathf.Clamp(levelReduction + intReduction, 0f, 0.8f)); // 最多减少80%消耗
        }

        /// <summary>
        /// 计算战斗资源恢复（基于精神和时间）
        /// </summary>
        public static float CalculateResourceRegen(float spirit, float deltaTime, float baseRegen = 1f)
        {
            return (baseRegen + spirit * 0.1f) * deltaTime;
        }

        /// <summary>
        /// 计算属性点重置成本（基于当前等级和已重置次数）
        /// </summary>
        public static int CalculateRespecCost(int playerLevel, int respecCount, int baseCost = 100)
        {
            return baseCost * playerLevel * (respecCount + 1);
        }

        /// <summary>
        /// 计算物品升级成本（基于物品等级和当前升级次数）
        /// </summary>
        public static int CalculateItemUpgradeCost(int itemLevel, int upgradeCount, int baseCost = 50)
        {
            return baseCost * itemLevel * (upgradeCount + 1);
        }

        /// <summary>
        /// 计算物品出售价格（基于物品价值和魅力）
        /// </summary>
        public static int CalculateItemSellPrice(int itemValue, int charisma, float baseMultiplier = 0.3f)
        {
            float charismaBonus = charisma * 0.01f;
            return Mathf.RoundToInt(itemValue * (baseMultiplier + charismaBonus));
        }

        /// <summary>
        /// 计算物品购买价格（基于物品价值和魅力）
        /// </summary>
        public static int CalculateItemBuyPrice(int itemValue, int charisma, float baseMultiplier = 1.2f)
        {
            float charismaDiscount = charisma * 0.005f;
            return Mathf.RoundToInt(itemValue * (baseMultiplier - Mathf.Clamp(charismaDiscount, 0f, 0.3f))); // 最多减少30%价格
        }
    }
}