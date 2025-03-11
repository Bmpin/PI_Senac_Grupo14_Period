using Backend___Grupo14.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;

public class FirebaseService
{
    private static FirebaseApp _firebaseApp;

    private static FirestoreDb _firestoreDb;
    public FirebaseService()
    {
        if (_firebaseApp == null)
        {
            var jsonContent = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS");

            if (string.IsNullOrEmpty(jsonContent))
            {
                throw new Exception("Credenciais do Firebase não encontradas nas variáveis de ambiente.");
            }

            _firebaseApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJson(jsonContent)
            });
        }

        if (_firestoreDb == null)
        {
            _firestoreDb = FirestoreDb.Create("grupo-14-pi");
        }
    }

    public async Task<string> RegisterUser(string email, string password)
    {
        var user = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
        {
            Email = email,
            Password = password
        });
        return user.Uid;
    }

    public async Task<string> LoginUser(string email, string password)
    {
        try
        {
            var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

            var token = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.Uid);
            return token;
        }
        catch (FirebaseAuthException ex)
        {
            throw new Exception("Usuário não encontrado ou erro na autenticação", ex);
        }
    }

    public async Task SaveDates(string userId, DateTime? data1, DateTime? data2, DateTime? data3)
    {
        try
        {
            var docRef = _firestoreDb.Collection("UserDates").Document(userId);
            var userData = new Dictionary<string, object>
        {
            { "Data1", data1 ?? null },
            { "Data2", data2 ?? null },
            { "Data3", data3 ?? null }
        };

            await docRef.SetAsync(userData, SetOptions.MergeAll);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao salvar datas no Firestore", ex);
        }
    }
    public async Task<DataCiclo> GetUserDates(string userId)
    {
        try
        {
            var docRef = _firestoreDb.Collection("UserDates").Document(userId);
            var docSnapshot = await docRef.GetSnapshotAsync();

            if (!docSnapshot.Exists)
            {
                return null;
            }

            var dataRecord = docSnapshot.ToDictionary();

            return new DataCiclo
            {
                Data1 = dataRecord.ContainsKey("Data1") && dataRecord["Data1"] is Timestamp ts1 ? ts1.ToDateTime() : null,
                Data2 = dataRecord.ContainsKey("Data2") && dataRecord["Data2"] is Timestamp ts2 ? ts2.ToDateTime() : null,
                Data3 = dataRecord.ContainsKey("Data3") && dataRecord["Data3"] is Timestamp ts3 ? ts3.ToDateTime() : null
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao recuperar datas no Firestore", ex);
        }
    }
}
