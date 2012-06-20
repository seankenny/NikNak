﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NikNak.Services
{
    public class BloodMonitorService : IBloodMonitorService
    {
        public ICollection<decimal> GetData()
        {
            // dummy data
            var values = "8.1903758020165,7.87662694775435,7.85737855178735,7.84967919340055,8.01521539871677,8.07488542621448,8.06333638863428,8.10375802016499,7.79770852428964,7.79385884509624,7.83428047662695,7.5263061411549,7.17983501374886,6.99890009165903,7.00274977085243,6.94307974335472,7.15288725939505,7.21833180568286,7.27222731439047,6.96425297891842,7.31264894592117,7.02007332722273,6.93345554537122,6.94500458295142,6.98542621448213,6.99312557286893,7.14133822181485,7.09706691109074,7.20870760769936,7.28570119156737,7.08551787351054,7.07974335472044,6.95270394133822,6.63703024747938,6.40027497708524,6.65435380384968,6.59660861594867,6.66782768102658,6.8160403299725,6.7756186984418,6.7890925756187,6.66012832263978,6.55811182401467,6.62163153070578,6.40797433547204,6.42144821264895,6.31365719523373,6.16736938588451,6.04417965169569,6.1,6.13464711274061,6.25976168652612,6.23281393217232,6.39835013748854,6.22896425297892,6.0711274060495,5.90366636113657,5.85939505041247,5.96141154903758,6.01915673693859,6.13272227314391,6.06150320806599,5.81704857928506,5.59761686526123,5.49560036663611,5.50329972502291,5.4340054995417,5.25114573785518,5.05096241979835,4.91814848762603,5.05673693858845,5.02016498625114,5.33968835930339,5.02786434463795,5.03171402383135,5.10293308890926,4.95472043996334,5.20687442713107,5.02593950504125,4.60439963336389,4.45811182401466,4.32722273143905,4.12318973418882,4.16361136571952,4.10586617781851,4.26562786434464,4.40229147571036,4.30027497708524,4.28295142071494,4.13666361136572,4.34839596700275,4.12318973418882,4.19633363886343,4.07891842346471,4.17516040329973,4.41961503208066,5.04326306141155,5.02593950504125,5.4301558203483,5.47442713107241,5.10485792850596,6.0980751604033,6.24821264894592,7.19330889092575,6.8141154903758,7.20100824931256,6.84106324472961,7.05279560036664,6.19046746104491,5.67653528872594,8.1884509624198,7.86700274977085,7.80155820348304,7.82850595783685,8.01329055912007,8.04793767186068,8.04986251145738,8.10568285976169,7.85737855178735,7.82273143904675,7.81695692025665,7.56095325389551,7.18175985334555,7.01237396883593,7.00467461044913,6.90265811182401,7.16251145737855,7.24142988084326,7.06626947754354,6.99890009165903,7.19138405132906,7.02007332722273,6.92383134738772,6.94692942254812,6.96810265811182,7.08166819431714,7.13556370302475,7.07781851512374,7.21448212648946,7.28762603116407,7.04702108157654,7.01814848762603,6.97965169569202,6.59275893675527,6.41182401466544,6.65435380384968,6.59083409715857,6.74097158570119,6.806416131989,6.83143904674611,6.8083409715857,6.58698441796517,6.55811182401467,6.59083409715857,6.39257561869844,6.36562786434464,6.29055912007333,6.13464711274061,6.05957836846929,6.1,6.1115490375802,6.25976168652612,6.18276810265811,6.37332722273144,6.23473877176902,6.0711274060495,5.90366636113657,5.91136571952337,5.91329055912007,6.0961503208066,6.0903758020165,6.05380384967919,5.78625114573786,5.59761686526123,5.52832263978002,5.51292392300642,5.47635197066911,5.24152153987168,5.11255728689276,4.89697525206233,5.09138405132906,5.04711274060495,5.32236480293309,5.12795600366636,4.94509624197984,5.18955087076077,4.89312557286893,5.20687442713107,5.02593950504125,4.61787351054079,4.46388634280477,4.40229147571036,4.11741521539872,4.09816681943171,4.10586617781851,4.29642529789184,4.36956920256645,4.27910174152154,4.33299725022915,4.14821264894592,4.33107241063245,4.12511457378552,4.19633363886343,4.08276810265811,4.19440879926673,4.62557286892759,5.01054078826764,4.98359303391384,5.4263061411549,5.49367552703941,5.11063244729606,6.1153987167736,6.23858845096242,7.31457378551787,6.90650779101742,7.21255728689276,6.7736938588451,7.07396883593034,5.96141154903758,5.63611365719523,8.1865261228231,7.82465627864345,7.80155820348304,7.80733272227314,8.10760769935839,8.08258478460128,7.94014665444546,8.06526122823098,7.91127406049496,7.82465627864345,7.79385884509624,7.53785517873511,7.20485792850596,6.88148487626031,7.00274977085243,6.98927589367553,7.16443629697525,7.17983501374886,6.98735105407883,7.03932172318973,7.13941338221815,7.01814848762603,6.91805682859762,6.94885426214482,6.93730522456462,7.12786434463795,7.09899175068744,7.10669110907424,7.21255728689276,7.25490375802017,7.04702108157654,6.99890009165903,6.94500458295142,6.57736021998167,6.42914757103575,6.64087992667278,6.58505957836847,6.74289642529789,6.825664527956,6.84106324472961,6.8102658111824,6.59275893675527,6.54656278643446,6.56196150320807,6.39257561869844,6.35600366636114,6.32520623281393,6.14234647112741,6.0692025664528,6.080751604033,6.17699358386801,6.25976168652612,6.17506874427131,6.31558203483043,6.23281393217232,6.0846012832264,5.89981668194317,5.96141154903758,5.91329055912007,6.19431714023831,6.01338221814849,6.0826764436297,5.80934922089826,5.58606782768103,5.57836846929423,5.51292392300642,5.44747937671861,5.20302474793767,5.13565536205316,4.88927589367553,5.06636113657195,5.03941338221815,5.32236480293309,5.06828597616865,4.96434463794684,5.22034830430797,4.88542621448213,5.17992667277727,4.99129239230064,4.61787351054079,4.42346471127406,4.40806599450046,4.19633363886343,4.12126489459212,4.07314390467461,4.30797433547204,4.36956920256645,4.18670944087993,4.31182401466544,4.30989917506874,4.27910174152154,4.13281393217232,4.16553620531622,4.08276810265811,4.28680109990834,4.76416131989001,5.02978918423465,5.01439046746104,5.4166819431714,5.32236480293309,5.11063244729606,6.05572868927589,6.29440879926673,7.34537121906508,7.03162236480293,7.19138405132906,6.85068744271311,7.07204399633364,5.54949587534372,5.65536205316224,8.2,7.89395050412466,7.66296975252062,7.88240146654445,8.10760769935839,8.10183318056828,7.91512373968836,8.00751604032997,7.99789184234647,7.85737855178735,7.83620531622365,7.5340054995417,7.17406049495875,6.85261228230981,6.91035747021082,6.91805682859762,7.19908340971586,7.17598533455545,6.98735105407883,7.04317140238314,7.09706691109074,7.01622364802933,7.00852428964253,6.87956003666361,6.93538038496792,7.12593950504125,7.11246562786434,7.12593950504125,7.24720439963336,7.18175985334555,7.06241979835014,6.99505041246563,6.94500458295142,6.54271310724106,6.51769019248396,6.58890925756187,6.55426214482127,6.73134738771769,6.8006416131989,6.84106324472961,6.75059578368469,6.58890925756187,6.50999083409716,6.58698441796517,6.40219981668194,6.37717690192484,6.32520623281393,6.1038496791934,6.03648029330889,6.1096241979835,6.19816681943171,6.24243813015582,6.21164069660862,6.31558203483043,6.25398716773602,6.06342804766269,5.90174152153987,5.98643446379468,5.87286892758937,6.28093492208983,6.01338221814849,6.0730522456462,5.81127406049496,5.58606782768103,5.55527039413382,5.49367552703941,5.4109074243813,5.19725022914757,5.06443629697525,4.91044912923923,5.03171402383135,5.15297891842346,5.31081576535289,5.00476626947754,4.96434463794684,5.11448212648946,4.94124656278643,5.17415215398717,4.92199816681943,4.60054995417049,4.29835013748854,4.40806599450046,4.25792850595784,4.11356553620532,4.0153987167736,4.39074243813016,4.38304307974336,4.14821264894592,4.31182401466544,4.35224564619615,4.26947754353804,4.14051329055912,4.09816681943171,4.06736938588451,4.27332722273144,4.76416131989001,4.98551787351054,5.05481209899175,5.55334555453712,5.3839596700275,5.25884509624198,6.01723189734189,6.29440879926673,6.99120073327223,7.07396883593034,7.17406049495875,6.92960586617782,6.93730522456462,5.26654445462878,5.65536205316224,8.13648029330889,7.79193400549954,7.59945004582951,7.88817598533456,8.11338221814849,8.08065994500458,7.91512373968836,7.93629697525206,7.93244729605866,7.73418881759853,7.82850595783685,7.493583868011,7.16828597616865,6.8295142071494,6.96810265811182,6.83913840513291,7.23950504124656,7.27030247479377,6.98735105407883,7.08744271310724,7.09706691109074,6.95847846012832,7.07204399633364,6.91805682859762,6.95847846012832,7.10476626947754,7.05857011915674,7.12401466544455,7.28377635197067,7.19330889092575,7.06626947754354,6.90650779101742,6.93730522456462,6.52731439046746,6.51769019248396,6.50421631530706,6.52346471127406,6.71209899175069,6.7775435380385,6.83143904674611,6.72557286892759,6.58890925756187,6.49651695692026,6.61393217231897,6.40797433547204,6.36947754353804,6.33675527039413,6.0903758020165,6.03648029330889,6.14619615032081,6.18854262144821,6.23281393217232,6.30788267644363,6.29633363886343,6.23473877176902,6.06342804766269,5.88249312557287,6.05572868927589,5.89404216315307,6.24821264894592,6.02300641613199,6.02493125572869,5.81127406049496,5.55334555453712,5.55527039413382,5.50522456461961,5.34353803849679,5.22227314390467,5.07406049495875,4.91429880843263,5.00861594867094,5.27809349220898,5.30696608615949,4.98936755270394,4.89890009165903,5.06828597616865,4.94124656278643,5.14142988084326,4.92007332722273,4.59477543538039,4.26370302474794,4.43308890925756,4.24445462878093,4.11934005499542,4,4.41576535288726,4.34647112740605,4.15591200733272,4.31952337305224,4.38111824014666,4.26947754353804,4.09816681943171,4.0153987167736,4.04812098991751,4.31952337305224,4.78725939505041,4.93739688359303,5.05481209899175,5.57451879010083,5.24344637946838,5.4012832263978,6.15197066911091,6.28093492208983,6.67937671860678,7.07011915673694,7.17598533455545,7.10476626947754,6.86608615948671,5.56681943171402,5.50137488542621,7.84005499541705,7.79193400549954,7.61292392300642,7.94014665444546,8.10760769935839,8.10183318056828,7.91319890009166,7.85737855178735,7.93244729605866,7.62254812098992,7.83428047662695,7.46471127406049,7.08166819431714,6.88340971585701,7.00659945004583,6.8372135655362,7.23758020164986,7.32997250229148,6.96810265811182,7.12786434463795,7.15288725939505,6.99505041246563,7.07204399633364,6.96810265811182,6.93538038496792,7.08359303391384,6.97387717690192,7.09321723189734,7.28955087076077,7.19523373052246,7.01622364802933,6.92960586617782,6.94885426214482,6.44454628780935,6.52731439046746,6.53693858845096,6.52346471127406,6.7698441796517,6.7929422548121,6.7852428964253,6.71787351054079,6.57543538038497,6.49844179651696,6.61393217231897,6.39065077910174,6.35215398716774,6.28863428047663,6.0769019248396,6.05380384967919,6.16736938588451,6.18661778185151,6.25783684692942,6.33098075160403,6.28285976168653,6.21934005499542,6.06150320806599,5.87479376718607,6.05572868927589,5.91521539871677,6.22318973418882,6.04610449129239,5.97296058661778,5.81127406049496,5.54179651695692,5.55527039413382,5.50522456461961,5.31081576535289,5.19532538955087,5.09330889092576,4.90467461044913,5.11640696608616,5.27809349220898,5.26846929422548,4.99321723189734,4.87965169569202,4.99899175068744,4.92584784601283,5.12218148487626,4.92007332722273,4.58707607699358,4.21750687442713,4.32529789184235,4.18285976168653,4.14051329055912,4.12703941338222,4.41769019248396,4.38689275893676,4.14821264894592,4.32914757103575,4.45618698441796,4.30604949587534,4.09431714023831,4.0134738771769,4.15206232813932,4.30412465627864,4.85270394133822,4.89697525206233,5.08368469294226,5.73428047662695,5.24152153987168,5.59569202566453,6.30595783684693,6.46379468377635,7.04317140238314,6.92383134738772,7.31072410632447,7.10476626947754,6.787167736022,5.77470210815765,5.58991750687443,7.82850595783685,7.80348304307974,7.64757103574702,7.94784601283226,8.09413382218148,8.14610449129239,8.04216315307058,7.74188817598534,7.93244729605866,7.61869844179652,7.83235563703025,7.42236480293309,7.06434463794684,6.95077910174152,6.99697525206233,6.8044912923923,7.19330889092575,7.32997250229148,7.01044912923923,7.15288725939505,7.18175985334555,6.94307974335472,7.04894592117324,6.98542621448213,6.93538038496792,7.10476626947754,6.99505041246563,7.04317140238314,7.27800183318057,7.23180568285976,7.01814848762603,6.92768102658112,6.87956003666361,6.36947754353804,6.46571952337305,6.53886342804766,6.52346471127406,6.806416131989,6.7948670944088,6.7794683776352,6.73519706691109,6.56581118240147,6.54463794683776,6.57736021998167,6.33098075160403,6.35215398716774,6.26938588450962,6.1019248395967,6.04802933088909,6.20201649862511,6.20394133822181,6.25398716773602,6.33098075160403,6.27131072410632,6.15389550870761,6.03070577451879,5.87671860678277,6.03070577451879,5.92676443629697,6.22318973418882,6.0730522456462,5.91906507791017,5.80934922089826,5.50137488542621,5.58221814848762,5.43978001833181,5.31081576535289,5.08753437213566,5.06636113657195,4.88927589367553,5.08560953253895,5.30119156736939,5.18955087076077,4.99321723189734,4.95279560036664,5.04903758020165,4.98359303391384,5.06636113657195,4.92199816681943,4.50623281393217,4.21750687442713,4.20788267644363,4.16746104491292,4.10009165902841,4.23675527039413,4.38111824014666,4.36956920256645,4.14821264894592,4.39266727772686,4.45233730522456,4.31182401466544,4.19440879926673,4.04234647112741,4.20980751604033,4.30412465627864,4.83923006416132,4.95087076076994,5.17030247479377,5.60916590284143,5.13565536205316,5.76892758936755,6.29440879926673,6.83528872593951,7.06819431714024,7.09129239230064,7.30879926672777,7.13748854262145,6.58120989917507,5.77277726856095,7.77846012832264,7.84197983501375,7.71686526122823,7.97286892758937,8.13648029330889,8.14610449129239,8.10183318056828,7.73033913840513,7.92089825847846,7.74188817598534,7.71494042163153,7.37616865261228,7.06434463794684,6.92383134738772,7.05279560036664,6.88148487626031,7.15866177818515,7.28185151237397,7.04124656278644,7.15673693858845,7.21063244729606,6.88918423464711,7.03739688359303,7.01429880843263,6.90458295142072,7.08359303391384,6.99697525206233,7.08166819431714,7.32227314390467,7.27415215398717,7.02392300641613,6.92190650779102,6.8025664527956,6.36947754353804,6.45032080659945,6.60045829514207,6.45994500458295,6.84876260311641,6.787167736022,6.71209899175069,6.73519706691109,6.58313473877177,6.52731439046746,6.52923923006416,6.38680109990834,6.34060494958753,6.23473877176902,6.1019248395967,6.0980751604033,6.17121906507791,6.23666361136572,6.25206232813932,6.35022914757104,6.30595783684693,6.15389550870761,5.93061411549037,5.85939505041247,5.97873510540788,6.00760769935839,6.20586617781852,6.03840513290559,5.91906507791017,5.71310724106324,5.4301558203483,5.63418881759853,5.4070577451879,5.28964252978918,5.06636113657195,5.06443629697525,4.94702108157654,5.07021081576535,5.29926672777269,5.06828597616865,5.01631530705774,5.06251145737855,5.04903758020165,5.08560953253895,5.04711274060495,4.89312557286893,4.44848762603116,4.13858845096242,4.11549037580202,4.16746104491292,4.06351970669111,4.25215398716774,4.33684692942255,4.35224564619615,4.14243813015582,4.38496791934006,4.45041246562786,4.28680109990834,4.19055912007333,4.0096241979835,4.19055912007333,4.28872593950504,4.86425297891842,4.94894592117324,5.23767186067828,5.46672777268561,4.97974335472044,5.85362053162236,6.1115490375802,6.94885426214482,7.06626947754354,7.27607699358387,7.23373052245646,7.14903758020165,6.36370302474794,5.70155820348304,7.78038496791934,7.83428047662695,7.84967919340055,8.00944087992667,8.11915673693859,8.1692025664528,8.07296058661778,7.73033913840513,7.96131989000917,7.85737855178735,7.62639780018332,7.26452795600367,7.04894592117324,7.00082493125573,7.05279560036664,6.95847846012832,7.16443629697525,7.22025664527956,6.98350137488543,7.22988084326306,7.06241979835014,6.88918423464711,6.96425297891842,7.03354720439963,6.93345554537122,7.09706691109074,6.99697525206233,7.08744271310724,7.32227314390467,7.24142988084326,7.07589367552704,6.94692942254812,6.72172318973419,6.38295142071494,6.51576535288726,6.60045829514207,6.46379468377635,6.8333638863428,6.76214482126489,6.73134738771769,6.75059578368469,6.60430797433547,6.53308890925756,6.51384051329056,6.42914757103575,6.33868010999083,6.17506874427131,6.1096241979835,6.0980751604033,6.16929422548121,6.23666361136572,6.24821264894592,6.35792850595784,6.28863428047663,6.1173235563703,5.91521539871677,5.85939505041247,6.01723189734189,5.99413382218149,6.21356553620532,6.02878093492209,5.89596700274977,5.66306141154904,5.4282309807516,5.55912007332722,5.3974335472044,5.29156736938588,5.03556370302475,5.06443629697525,4.94702108157654,5.07021081576535,5.30311640696609,5.01054078826764,5.02208982584785,5.08175985334556,5.01631530705774,5.12988084326306,5.04711274060495,4.73336388634281,4.47158570119157,4.14436296975252,4.12896425297892,4.19055912007333,4.0173235563703,4.25407882676444,4.38111824014666,4.29450045829514,4.14821264894592,4.22520623281393,4.40806599450046,4.19633363886343,4.19055912007333,4.04619615032081,4.19055912007333,4.34069660861595,4.99129239230064,4.97781851512374,5.24152153987168,5.46287809349221,5.05096241979835,5.91136571952337,6.02108157653529,7.14518790100825,6.98735105407883,7.33959670027498,7.23180568285976,7.11824014665445,6.31365719523373,5.70733272227314,7.82850595783685,7.84967919340055,7.84967919340055,8.01906507791017,8.07103574702108,8.14417965169569,8.10183318056828,7.74573785517874,7.87470210815765,7.85737855178735,7.5186067827681,7.17598533455545,7.07589367552704,6.97772685609532,7.03354720439963,7.04317140238314,7.16251145737855,7.25490375802017,6.96425297891842,7.30687442713107,7.08166819431714,6.89303391384051,6.90843263061412,7.03162236480293,6.96232813932172,7.13941338221815,7.06241979835014,7.14133822181485,7.32034830430797,7.15288725939505,7.07589367552704,6.96040329972502,6.64087992667278,6.37910174152154,6.59275893675527,6.59468377635197,6.58698441796517,6.8333638863428,6.73519706691109,6.7890925756187,6.75252062328139,6.59083409715857,6.58698441796517,6.48881759853345,6.42914757103575,6.35407882676444,6.16736938588451,6.1038496791934,6.1230980751604,6.14812098991751,6.25013748854262,6.24821264894592,6.41182401466544,6.22896425297892,6.0711274060495,5.89789184234647,5.85939505041247,6.01338221814849,5.99413382218149,6.20394133822181,6.06150320806599,5.87286892758937,5.60916590284143,5.44362969752521,5.50907424381302,5.3955087076077,5.25692025664528,5.05096241979835,5.00669110907424,4.99321723189734,5.05288725939505,5.33776351970669,5.01054078826764,5.02786434463795,5.10293308890926,4.96241979835014,5.22034830430797,5.06443629697525,4.65829514207149,4.47158570119157,4.20980751604033,4.12318973418882,4.16938588450962,4.07506874427131,4.24445462878093,4.46196150320807,4.29450045829514,4.21173235563703,4.16746104491292,4.35032080659945,4.15398716773602,4.21943171402383,4.08084326306141,4.19055912007333,4.38304307974336,5.04326306141155,5.05096241979835,5.4243813015582,5.4186067827681,5.05096241979835,5.91906507791017,6.1019248395967,7.09321723189734,7.05857011915674,7.05664527956004,7.10476626947754,7.09899175068744,6.31750687442713,5.71118240146654,8.2";
            var test = values.Split(',').Select(s => Convert.ToDecimal(s)).ToList();

            return test;
        }
    }
}