using UnityEngine;
using UnityEditor;

public partial class Protocol
{
    public static string token;

    public enum eSortType
    {
        None = -1,
        Favorite,
        Clear,
        ElapsedScore,
        CreatedAt,
    }

    public class req_stage_info
    {
        public string title;        // 타이틀
        public string user_id;      // 만든이
        public int map_id;          // 맵 id
    }

    public class res_stage_info
    {
        public int stage_id;        // 스테이지 id
        public string title;        // 타이틀
        public string user_id;      // 유저 이메일
        public int favorite_count;  // 좋아요
        public int clear_count;     // 클리어 횟수
        public int elapsed_score;   // 누적 점수
        public string created_at;   // 생성일
        public int map_id;          // 맵 id
        public string user_name;   // 유저 닉네임
    }

    public class req_user_info
    {
        public string user_id;      // 유저 id
        public string user_name;    // 유저 닉네임
    }

    public class res_user_info
    {
        public string user_id;      // 유저ID
        public int score;           // 유저 점수
        public int rank;            // 유저 랭킹
        public string user_name;    // 유저 이름
        public string country;      //유저 국적
    }

    public class req_common
    {
        public int cmd;     // 명령어
    }

    public class res_common
    {
        public int cmd;         //상태코드 
        public string message;  //결과 문자열 
        public int status;      // http status code
    }

    // 로그인
    public class req_login : req_common
    {
        public string id;
        public string userName;
        public string deviceId;
        public string country;

        public override string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}", id, userName, deviceId, country);
        }
    }

    public class res_login : res_common
    {
        public string token;
    }

    // 스테이지 목록 조회
    public class req_getStages : req_common
    {
        public int sort_type;
    }

    public class res_getStages : res_common
    {
        public res_stage_info[] arrStageInfo;   // 스테이지 정보들
    }

    public class req_doSaveStage : req_common
    {
        public req_stage_info stageInfo;       // 스테이지 정보
        public unit_info[] arrUnitInfo;        // 유닛 정보들
    }

    public class res_doSaveStage : res_common
    {
    }

    public class req_doPlay : req_common
    {
        public int stage_id;
    }

    public class res_doPlay : res_common
    {
        public unit_info[] arrUnitInfo;
    }

    public class req_getRank : req_common
    {
    }

    public class res_getRank : res_common
    {
        public res_user_info[] arrUserInfo;
    }

    public class req_getMyRank : req_common
    {
        public string user_id;
    }

    public class res_getMyRank : res_common
    {
        public res_user_info[] userInfo;
    }
    public class req_doFavoriteStage : req_common
    {
        public int stage_id;
    }

    public class res_doFavoriteStage : res_common
    {
    }

    public class req_clearStage : req_common
    {
        public int stage_id;
        public string user_id;
        public int stage_elapsed_score;
    }

    public class res_clearStage : res_common
    {
    }

    public class req_failedStage : req_common
    {
        public int stage_id;
    }

    public class res_failedStage : res_common
    {
    }

}