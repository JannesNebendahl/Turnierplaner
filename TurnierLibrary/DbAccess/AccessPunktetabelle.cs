using Dapper;
using DemoLibary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;


namespace TurnierLibrary.DbAccess
{
    public class AccessPunktetabelle : SqliteDataAccess
    {
        public static List<Punktetabelle> LoadPunktetabelle()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string sql = @"select M.Name,
                            sum(M.Id == Punkterechnung.Heim or M.Id == Punkterechnung.Aus) as Spiele,
                               sum(iif(Punkterechnung.Heim == M.Id and Punkterechnung.Punkte == 3, 1,
                                   iif(Punkterechnung.Aus == M.Id and Punkterechnung.Punkte == 0, 1, 0))
                                   ) as Siege,
                               sum(iif(Punkterechnung.Heim == M.Id and Punkterechnung.Punkte == 0, 1,
                                   iif(Punkterechnung.Aus == M.Id and Punkterechnung.Punkte == 3, 1, 0))
                                   ) as Niederlagen,
                               sum(iif(Punkterechnung.Heim == M.Id and Punkterechnung.Punkte == 1, 1,
                                   iif(Punkterechnung.Aus == M.Id and Punkterechnung.Punkte == 1, 1, 0))
                                   ) as Unentschieden,
                            sum(iif(Punkterechnung.Heim == M.Id , Punkterechnung.Heimtore,
                                   iif(Punkterechnung.Aus == M.Id, Punkterechnung.Austore, 0))
                                   ) || ':' ||
                                sum(iif(Punkterechnung.Heim == M.Id , Punkterechnung.Austore,
                                   iif(Punkterechnung.Aus == M.Id, Punkterechnung.Heimtore, 0))
                                   )
                            as Tordifferenz,
                               sum(iif(Punkterechnung.Heim == M.Id, Punkterechnung.Punkte,
                            case when Punkterechnung.Aus == M.Id then
                                    case when Punkterechnung.Punkte == 3 then 0
                            when Punkterechnung.Punkte == 1 then 1
                            else 3
                            end
                            end
                            )) as Punkte
                        from Mannschaften M,(
                        select H.Id as Heim,
                                sum(CASE  WHEN (T.SpielID == S.Id AND T.Mannschaft == H.Id and T.Typ != 2) THEN 1
                                       when (T.SpielID == S.Id AND T.Mannschaft == A.Id and T.Typ == 2) then 1
                                       else 0 END) as Heimtore,
                                sum(CASE  WHEN (T.SpielID == S.Id AND T.Mannschaft == A.Id and T.Typ != 2) THEN 1
                                       when (T.SpielID == S.Id AND T.Mannschaft == H.Id and T.Typ == 2) then 1
                                       else 0 END) as Austore,
                               A.Id as Aus,
                               iif(H.Id == S.HeimmannschaftsId,
                               iif(
                                   sum(CASE  WHEN (T.SpielID == S.Id AND T.Mannschaft == H.Id and T.Typ != 2) THEN 1
                                       when (T.SpielID == S.Id AND T.Mannschaft == A.Id and T.Typ == 2) then 1
                                       else 0 END)
                                       >
                                   sum(CASE  WHEN (T.SpielID == S.Id AND T.Mannschaft == A.Id and T.Typ != 2) THEN 1
                                       when (T.SpielID == S.Id AND T.Mannschaft == H.Id and T.Typ == 2) then 1
                                       else 0 END), 3,
                                   iif(sum(CASE  WHEN (T.SpielID == S.Id AND T.Mannschaft == H.Id and T.Typ != 2) THEN 1
                                       when (T.SpielID == S.Id AND T.Mannschaft == A.Id and T.Typ == 2) then 1
                                       else 0 END) ==
                                   sum(CASE  WHEN (T.SpielID == S.Id AND T.Mannschaft == A.Id and T.Typ != 2) THEN 1
                                       when (T.SpielID == S.Id AND T.Mannschaft == H.Id and T.Typ == 2) then 1
                                       else 0 END), 1, 0
                                   )),
                                   0) as Punkte
                        from Spiel S, Tor T, Mannschaften H, Mannschaften A
                        where H.Id == S.HeimmannschaftsId and A.Id == S.AuswaertsmannschaftsId
                        group by S.Id) as Punkterechnung
                        group by M.Id
                        order by Punkte desc;";
                var output = cnn.Query<Punktetabelle>(sql, new DynamicParameters());
                return output.AsList();
            }
        }
    }
}
