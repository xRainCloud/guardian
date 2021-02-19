using UnityEngine;

public class CustomCharacterManager : MonoBehaviour
{
    private HERO_SETUP setup;
    public GameObject character;
    public GameObject labelSex;
    public GameObject labelEye;
    public GameObject labelFace;
    public GameObject labelGlass;
    public GameObject labelHair;
    public GameObject labelCostume;
    public GameObject labelCape;
    public GameObject labelDivision;
    public GameObject labelSkin;
    public GameObject labelPreset;
    public GameObject labelPOINT;
    public GameObject labelSPD;
    public GameObject labelGAS;
    public GameObject labelBLA;
    public GameObject labelACL;
    public GameObject labelSKILL;
    public GameObject hairR;
    public GameObject hairG;
    public GameObject hairB;
    private Sex[] sexOption;
    private int sexId;
    private int[] eyeOption;
    private int eyeId;
    private int[] faceOption;
    private int faceId;
    private int[] glassOption;
    private int glassId;
    private int[] hairOption;
    private int hairId;
    private int[] skinOption;
    private int skinId;
    private HeroCostume[] costumeOption;
    private int costumeId = 1;
    private int[] capeOption;
    private int capeId;
    private Division[] divisionOption;
    private int divisionId;
    private int presetId;
    private string[] skillOption;
    private int skillId;
    private string currentSlot = "Set 1";

    private void Start()
    {
        QualitySettings.SetQualityLevel(5, applyExpensiveChanges: true);
        costumeOption = HeroCostume.CostumeOptions;
        setup = character.GetComponent<HERO_SETUP>();
        setup.Init();
        setup.myCostume = new HeroCostume();
        copyCostume(HeroCostume.Costumes[2], setup.myCostume);
        setup.myCostume.SetMesh();
        setup.CreateCharacterComponent();
        sexOption = new Sex[2]
        {
            Sex.Male,
            Sex.Female
        };
        eyeOption = new int[28];
        for (int i = 0; i < 28; i++)
        {
            eyeOption[i] = i;
        }
        faceOption = new int[14];
        for (int i = 0; i < 14; i++)
        {
            faceOption[i] = i + 32;
        }
        glassOption = new int[10];
        for (int i = 0; i < 10; i++)
        {
            glassOption[i] = i + 48;
        }
        hairOption = new int[11];
        for (int i = 0; i < 11; i++)
        {
            hairOption[i] = i;
        }
        skinOption = new int[3];
        for (int i = 0; i < 3; i++)
        {
            skinOption[i] = i + 1;
        }
        capeOption = new int[2];
        for (int i = 0; i < 2; i++)
        {
            capeOption[i] = i;
        }
        divisionOption = new Division[4]
        {
            Division.TraineesSquad,
            Division.TheGarrison,
            Division.TheMilitaryPolice,
            Division.TheSurveryCorps
        };
        skillOption = new string[8]
        {
            "mikasa",
            "levi",
            "sasha",
            "jean",
            "marco",
            "armin",
            "petra",
            "eren"
        };
        CostumeDataToMyID();
        freshLabel();
    }

    private int toNext(int id, int Count, int start = 0)
    {
        id++;
        if (id >= Count)
        {
            id = start;
        }
        id = Mathf.Clamp(id, start, start + Count - 1);
        return id;
    }

    private int toPrev(int id, int Count, int start = 0)
    {
        id--;
        if (id < start)
        {
            id = Count - 1;
        }
        id = Mathf.Clamp(id, start, start + Count - 1);
        return id;
    }

    public void nextOption(CreatePart part)
    {
        if (part == CreatePart.Preset)
        {
            presetId = toNext(presetId, HeroCostume.Costumes.Length);
            copyCostume(HeroCostume.Costumes[presetId], setup.myCostume, init: true);
            CostumeDataToMyID();
            setup.DeleteCharacterComponent();
            setup.CreateCharacterComponent();
            labelPreset.GetComponent<UILabel>().text = HeroCostume.Costumes[presetId].name;
            freshLabel();
        }
        else
        {
            toOption2(part, next: true);
        }
    }

    public void prevOption(CreatePart part)
    {
        if (part == CreatePart.Preset)
        {
            presetId = toPrev(presetId, HeroCostume.Costumes.Length);
            copyCostume(HeroCostume.Costumes[presetId], setup.myCostume, init: true);
            CostumeDataToMyID();
            setup.DeleteCharacterComponent();
            setup.CreateCharacterComponent();
            labelPreset.GetComponent<UILabel>().text = HeroCostume.Costumes[presetId].name;
            freshLabel();
        }
        else
        {
            toOption2(part, next: false);
        }
    }

    private void copyCostume(HeroCostume from, HeroCostume to, bool init = false)
    {
        copyBodyCostume(from, to);
        to.sex = from.sex;
        to.hair_mesh = from.hair_mesh;
        to.hair_1_mesh = from.hair_1_mesh;
        to.hair_color = new Color(from.hair_color.r, from.hair_color.g, from.hair_color.b);
        to.hairInfo = from.hairInfo;
        to.cape = from.cape;
        to.cape_mesh = from.cape_mesh;
        to.cape_texture = from.cape_texture;
        to.brand1_mesh = from.brand1_mesh;
        to.brand2_mesh = from.brand2_mesh;
        to.brand3_mesh = from.brand3_mesh;
        to.brand4_mesh = from.brand4_mesh;
        to.brand_texture = from.brand_texture;
        to._3dmg_texture = from._3dmg_texture;
        to.face_texture = from.face_texture;
        to.eye_mesh = from.eye_mesh;
        to.glass_mesh = from.glass_mesh;
        to.beard_mesh = from.beard_mesh;
        to.eye_texture_id = from.eye_texture_id;
        to.beard_texture_id = from.beard_texture_id;
        to.glass_texture_id = from.glass_texture_id;
        to.skin_color = from.skin_color;
        to.skin_texture = from.skin_texture;
        to.beard_texture_id = from.beard_texture_id;
        to.hand_l_mesh = from.hand_l_mesh;
        to.hand_r_mesh = from.hand_r_mesh;
        to.mesh_3dmg = from.mesh_3dmg;
        to.mesh_3dmg_gas_l = from.mesh_3dmg_gas_l;
        to.mesh_3dmg_gas_r = from.mesh_3dmg_gas_r;
        to.mesh_3dmg_belt = from.mesh_3dmg_belt;
        to.weapon_l_mesh = from.weapon_l_mesh;
        to.weapon_r_mesh = from.weapon_r_mesh;
        if (init)
        {
            to.stat = new HeroStat();
            to.stat.Accel = 110;
            to.stat.Speed = 110;
            to.stat.Gas = 113;
            to.stat.Blade = 110;
            to.stat.SkillId = "mikasa";
        }
        else
        {
            to.stat = new HeroStat();
            to.stat.Accel = from.stat.Accel;
            to.stat.Speed = from.stat.Speed;
            to.stat.Gas = from.stat.Gas;
            to.stat.Blade = from.stat.Blade;
            to.stat.SkillId = from.stat.SkillId;
        }
    }

    private void copyBodyCostume(HeroCostume from, HeroCostume to)
    {
        to.arm_l_mesh = from.arm_l_mesh;
        to.arm_r_mesh = from.arm_r_mesh;
        to.body_mesh = from.body_mesh;
        to.body_texture = from.body_texture;
        to.uniform_type = from.uniform_type;
        to.part_chest_1_object_mesh = from.part_chest_1_object_mesh;
        to.part_chest_1_object_texture = from.part_chest_1_object_texture;
        to.part_chest_object_mesh = from.part_chest_object_mesh;
        to.part_chest_object_texture = from.part_chest_object_texture;
        to.part_chest_skinned_cloth_mesh = from.part_chest_skinned_cloth_mesh;
        to.part_chest_skinned_cloth_texture = from.part_chest_skinned_cloth_texture;
        to.division = from.division;
        to.id = from.id;
        to.costumeId = from.costumeId;
    }

    public void OnHairRChange(float value)
    {
        if (setup.myCostume != null && setup.part_hair != null)
        {
            HeroCostume myCostume = setup.myCostume;
            Color color = setup.part_hair.renderer.material.color;
            float g = color.g;
            Color color2 = setup.part_hair.renderer.material.color;
            myCostume.hair_color = new Color(value, g, color2.b);
            setHairColor();
        }
    }

    public void OnHairGChange(float value)
    {
        if (setup.myCostume != null && setup.part_hair != null)
        {
            HeroCostume myCostume = setup.myCostume;
            Color color = setup.part_hair.renderer.material.color;
            float r = color.r;
            Color color2 = setup.part_hair.renderer.material.color;
            myCostume.hair_color = new Color(r, value, color2.b);
            setHairColor();
        }
    }

    public void OnHairBChange(float value)
    {
        if (setup != null && setup.myCostume != null && setup.part_hair != null)
        {
            HeroCostume myCostume = setup.myCostume;
            Color color = setup.part_hair.renderer.material.color;
            float r = color.r;
            Color color2 = setup.part_hair.renderer.material.color;
            myCostume.hair_color = new Color(r, color2.g, value);
            setHairColor();
        }
    }

    private void setHairColor()
    {
        if (setup.part_hair != null)
        {
            setup.part_hair.renderer.material.color = setup.myCostume.hair_color;
        }
        if (setup.part_hair_1 != null)
        {
            setup.part_hair_1.renderer.material.color = setup.myCostume.hair_color;
        }
    }

    private void freshLabel()
    {
        labelSex.GetComponent<UILabel>().text = sexOption[sexId].ToString();
        labelEye.GetComponent<UILabel>().text = "eye_" + eyeId.ToString();
        labelFace.GetComponent<UILabel>().text = "face_" + faceId.ToString();
        labelGlass.GetComponent<UILabel>().text = "glass_" + glassId.ToString();
        labelHair.GetComponent<UILabel>().text = "hair_" + hairId.ToString();
        labelSkin.GetComponent<UILabel>().text = "skin_" + skinId.ToString();
        labelCostume.GetComponent<UILabel>().text = "costume_" + costumeId.ToString();
        labelCape.GetComponent<UILabel>().text = "cape_" + capeId.ToString();
        labelDivision.GetComponent<UILabel>().text = divisionOption[divisionId].ToString();
        labelPOINT.GetComponent<UILabel>().text = "Points: " + (450 - calTotalPoints()).ToString();
        labelSPD.GetComponent<UILabel>().text = "SPD " + setup.myCostume.stat.Speed.ToString();
        labelGAS.GetComponent<UILabel>().text = "GAS " + setup.myCostume.stat.Gas.ToString();
        labelBLA.GetComponent<UILabel>().text = "BLA " + setup.myCostume.stat.Blade.ToString();
        labelACL.GetComponent<UILabel>().text = "ACL " + setup.myCostume.stat.Accel.ToString();
        labelSKILL.GetComponent<UILabel>().text = "SKILL " + setup.myCostume.stat.SkillId.ToString();
    }

    private void CostumeDataToMyID()
    {
        int num = 0;
        for (num = 0; num < sexOption.Length; num++)
        {
            if (sexOption[num] == setup.myCostume.sex)
            {
                sexId = num;
                break;
            }
        }
        for (num = 0; num < eyeOption.Length; num++)
        {
            if (eyeOption[num] == setup.myCostume.eye_texture_id)
            {
                eyeId = num;
                break;
            }
        }
        faceId = -1;
        for (num = 0; num < faceOption.Length; num++)
        {
            if (faceOption[num] == setup.myCostume.beard_texture_id)
            {
                faceId = num;
                break;
            }
        }
        glassId = -1;
        for (num = 0; num < glassOption.Length; num++)
        {
            if (glassOption[num] == setup.myCostume.glass_texture_id)
            {
                glassId = num;
                break;
            }
        }
        for (num = 0; num < hairOption.Length; num++)
        {
            if (hairOption[num] == setup.myCostume.hairInfo.id)
            {
                hairId = num;
                break;
            }
        }
        for (num = 0; num < skinOption.Length; num++)
        {
            if (skinOption[num] == setup.myCostume.skin_color)
            {
                skinId = num;
                break;
            }
        }
        if (setup.myCostume.cape)
        {
            capeId = 1;
        }
        else
        {
            capeId = 0;
        }
        for (num = 0; num < divisionOption.Length; num++)
        {
            if (divisionOption[num] == setup.myCostume.division)
            {
                divisionId = num;
                break;
            }
        }
        costumeId = setup.myCostume.costumeId;
        float r = setup.myCostume.hair_color.r;
        float g = setup.myCostume.hair_color.g;
        float b = setup.myCostume.hair_color.b;
        hairR.GetComponent<UISlider>().sliderValue = r;
        hairG.GetComponent<UISlider>().sliderValue = g;
        hairB.GetComponent<UISlider>().sliderValue = b;
        num = 0;
        while (true)
        {
            if (num < skillOption.Length)
            {
                if (skillOption[num] == setup.myCostume.stat.SkillId)
                {
                    break;
                }
                num++;
                continue;
            }
            return;
        }
        skillId = num;
    }

    public void nextStatOption(CreateStat type)
    {
        if (type == CreateStat.Skill)
        {
            skillId = toNext(skillId, skillOption.Length);
            setup.myCostume.stat.SkillId = skillOption[skillId];
            character.GetComponent<CharacterCreateAnimationControl>().playAttack(setup.myCostume.stat.SkillId);
            freshLabel();
        }
        else if (calTotalPoints() < 450)
        {
            setStatPoint(type, 1);
        }
    }

    public void prevStatOption(CreateStat type)
    {
        if (type == CreateStat.Skill)
        {
            skillId = toPrev(skillId, skillOption.Length);
            setup.myCostume.stat.SkillId = skillOption[skillId];
            character.GetComponent<CharacterCreateAnimationControl>().playAttack(setup.myCostume.stat.SkillId);
            freshLabel();
        }
        else
        {
            setStatPoint(type, -1);
        }
    }

    private void setStatPoint(CreateStat type, int pt)
    {
        switch (type)
        {
            case CreateStat.SPD:
                setup.myCostume.stat.Speed += pt;
                break;
            case CreateStat.GAS:
                setup.myCostume.stat.Gas += pt;
                break;
            case CreateStat.BLA:
                setup.myCostume.stat.Blade += pt;
                break;
            case CreateStat.ACL:
                setup.myCostume.stat.Accel += pt;
                break;
        }

        setup.myCostume.stat.Speed = Mathf.Clamp(setup.myCostume.stat.Speed, 75, 125);
        setup.myCostume.stat.Gas = Mathf.Clamp(setup.myCostume.stat.Gas, 75, 125);
        setup.myCostume.stat.Blade = Mathf.Clamp(setup.myCostume.stat.Blade, 75, 125);
        setup.myCostume.stat.Accel = Mathf.Clamp(setup.myCostume.stat.Accel, 75, 125);

        freshLabel();
    }

    private int calTotalPoints()
    {
        if (setup.myCostume != null)
        {
            int num = 0;
            num += setup.myCostume.stat.Speed;
            num += setup.myCostume.stat.Gas;
            num += setup.myCostume.stat.Blade;
            return num + setup.myCostume.stat.Accel;
        }
        return 450;
    }

    public void SaveData()
    {
        CostumeConverter.ToLocalData(setup.myCostume, currentSlot);
    }

    public void LoadData()
    {
        HeroCostume heroCostume = CostumeConverter.FromLocalData(currentSlot);
        if (heroCostume != null)
        {
            copyCostume(heroCostume, setup.myCostume);
            setup.DeleteCharacterComponent();
            setup.CreateCharacterComponent();
        }
        CostumeDataToMyID();
        freshLabel();
    }

    public void OnSoltChange(string id)
    {
        currentSlot = id;
    }

    public void toOption2(CreatePart part, bool next)
    {
        switch (part)
        {
            case CreatePart.Sex:
                sexId = ((!next) ? toPrev(sexId, sexOption.Length) : toNext(sexId, sexOption.Length));
                if (sexId != 0)
                {
                    costumeId = 0;
                }
                else
                {
                    costumeId = 11;
                }
                copyCostume(costumeOption[costumeId], setup.myCostume, init: true);
                setup.myCostume.sex = sexOption[sexId];
                character.GetComponent<CharacterCreateAnimationControl>().toStand();
                CostumeDataToMyID();
                setup.DeleteCharacterComponent();
                setup.CreateCharacterComponent();
                break;
            case CreatePart.Eye:
                eyeId = ((!next) ? toPrev(eyeId, eyeOption.Length) : toNext(eyeId, eyeOption.Length));
                setup.myCostume.eye_texture_id = eyeId;
                setup.SetFaceTexture(setup.part_eye, eyeOption[eyeId]);
                break;
            case CreatePart.Face:
                faceId = ((!next) ? toPrev(faceId, faceOption.Length) : toNext(faceId, faceOption.Length));
                setup.myCostume.beard_texture_id = faceOption[faceId];
                if (setup.part_face == null)
                {
                    setup.CreateFace();
                }
                setup.SetFaceTexture(setup.part_face, faceOption[faceId]);
                break;
            case CreatePart.Glass:
                glassId = ((!next) ? toPrev(glassId, glassOption.Length) : toNext(glassId, glassOption.Length));
                setup.myCostume.glass_texture_id = glassOption[glassId];
                if (setup.part_glass == null)
                {
                    setup.CreateGlasses();
                }
                setup.SetFaceTexture(setup.part_glass, glassOption[glassId]);
                break;
            case CreatePart.Hair:
                hairId = ((!next) ? toPrev(hairId, hairOption.Length) : toNext(hairId, hairOption.Length));
                if (sexId != 0)
                {
                    setup.myCostume.hair_mesh = CostumeHair.FemaleHairs[hairOption[hairId]].hair;
                    setup.myCostume.hair_1_mesh = CostumeHair.FemaleHairs[hairOption[hairId]].hair_1;
                    setup.myCostume.hairInfo = CostumeHair.FemaleHairs[hairOption[hairId]];
                }
                else
                {
                    setup.myCostume.hair_mesh = CostumeHair.MaleHairs[hairOption[hairId]].hair;
                    setup.myCostume.hair_1_mesh = CostumeHair.MaleHairs[hairOption[hairId]].hair_1;
                    setup.myCostume.hairInfo = CostumeHair.MaleHairs[hairOption[hairId]];
                }
                setup.CreateHair();
                setHairColor();
                break;
            case CreatePart.Skin:
                skinId = ((!next) ? toPrev(skinId, 2) : toNext(skinId, 2));
                if (setup.myCostume.uniform_type == UNIFORM_TYPE.CasualAHSS && skinId == 0)
                {
                    skinId = 2;
                }

                setup.myCostume.skin_color = skinOption[skinId];
                setup.myCostume.setTexture();
                setup.SetSkin();
                break;
            case CreatePart.Costume:
                if (setup.myCostume.uniform_type != UNIFORM_TYPE.CasualAHSS)
                {
                    costumeId = !next ? toPrev(costumeId, 24) : toNext(costumeId, 24);
                } else
                {
                    costumeId = 25;
                }
                copyBodyCostume(costumeOption[costumeId], setup.myCostume);
                setup.myCostume.SetMesh();
                setup.myCostume.setTexture();
                setup.CreateUpperBody();
                setup.CreateLeftArm();
                setup.CreateRightArm();
                setup.CreateLowerBody();
                break;
            case CreatePart.Cape:
                capeId = ((!next) ? toPrev(capeId, capeOption.Length) : toNext(capeId, capeOption.Length));
                setup.myCostume.cape = (capeId == 1);
                setup.myCostume.setCape();
                setup.myCostume.setTexture();
                setup.CreateCape();
                break;
            case CreatePart.Division:
                divisionId = ((!next) ? toPrev(divisionId, divisionOption.Length) : toNext(divisionId, divisionOption.Length));
                setup.myCostume.division = divisionOption[divisionId];
                setup.myCostume.setTexture();
                setup.CreateUpperBody();
                break;
        }
        freshLabel();
    }
}
