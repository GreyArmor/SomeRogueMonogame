using System;

namespace NamelessRogue.Engine.Generation
{
    public class ChineseNamesGenerator : ICharacterNameGenerator
    {
        string[] surnames;
        string[] maleGivenNames;
        string[] femaleGivenNames;
        public ChineseNamesGenerator()
        {
            surnames = new string[]
            {
               "Wang", "Li", "Zhang", "Liu", "Chen",
               "Yang", "Huang", "Zhao", "Wu", "Zhou",
               "Xu", "Sun", "Ma", "Zhu", "Hu",
               "Guo", "He", "Gao", "Lin", "Luo",
               "Zheng", "Liang", "Xie", "Song", "Tang",
               "Han", "Feng", "Cao", "Peng", "Pan",
               "Yu", "Deng", "Wei", "Jiang", "Shen",
               "Yao", "Ren", "Lu", "Shao", "Tian",
               "Fan", "Hao", "Meng", "Qin", "Bai",
               "Cui", "Kang", "Shi", "Hou", "Mao"
            };
                
            // Common male given names (2-syllable style)
            maleGivenNames = new string[]
            {
                "Wei", "Jun", "Lei", "Hao", "Jian",
                "Peng", "Bo", "Feng", "Gang", "Qiang",
                "Chao", "Ming", "Rui", "Tao", "Yong",
                "Guang", "Hui", "Jie", "Sheng", "Xin",
                "Zhihui", "Zhiqiang", "Xiaolong", "Haoran", "Yuchen",
                "Juncheng", "Zihan", "Zixuan", "Haoyu", "Zeyu",
                "Yuze", "Shuai", "Tianyu", "Wenhao", "Yiming",
                "Yifan", "Ziheng", "Zhiyuan", "Jianhua", "Hailong",
                "Yunhao", "Hongwei", "Yuepeng", "Xiaobo", "Zhonghua",
                "Jinwei", "Zhendong", "Weidong", "Shaohua", "Donghai"
            };

            // Common female given names (2-syllable style)
            maleGivenNames = new string[]
            {
                "Li", "Hua", "Fang", "Mei", "Lan",
                "Ying", "Jie", "Xiu", "Na", "Yan",
                "Ling", "Fen", "Xia", "Qin", "Yun",
                "Lihua", "Meiling", "Xiaomei", "Huifang", "Yingying",
                "Xiaoling", "Qiuyue", "Ronghua", "Xiaohong", "Yulan",
                "Zhilan", "Yanling", "Xiaoyu", "Huiling", "Xuefang",
                "Jingyi", "Yueying", "Shanshan", "Zhenzhen", "Liping",
                "Huanhuan", "Xinxin", "Yaqin", "Minmin", "Xiaoqing",
                "Zhaohui", "Guifang", "Xiumei", "Qiaolian", "Wenhui",
                "Xiaolan", "Yanmei", "Xiaoyun", "Jinghua", "Meifen"
            };
        }

        public string GenerateName(bool isMale)
        {
            Random rnd = new Random();
            string surname = surnames[rnd.Next(surnames.Length)];
            if (isMale)
            {
                string name = maleGivenNames[rnd.Next(femaleGivenNames.Length)];
                return surname + " " + name;
            }
            else
            {
                string name = femaleGivenNames[rnd.Next(femaleGivenNames.Length)];
                return surname + " " + name;
            }
        }
    }
}
