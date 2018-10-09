using Moq;
using StackExchange.StacMan.Tests.Utilities;
using System;
using System.Linq;
using Xunit;

namespace StackExchange.StacMan.Tests
{
    public class ApiVersion22Tests
    {
        [Fact]
        public void Answer_tags_test()
        {
            var mock22 = new Mock<StacManClient>(null, "2.2");

            //http://api.stackexchange.com/2.2/answers?pagesize=1&order=desc&sort=activity&site=stackoverflow&filter=!9hnGsz84b
            mock22.FakeGET(response: @"{""items"":[{""tags"":[""django"",""youtube-dl"",""pafy""],""owner"":{""reputation"":448,""user_id"":9321755,""user_type"":""registered"",""profile_image"":""https://lh6.googleusercontent.com/-MJdCwK_eJfw/AAAAAAAAAAI/AAAAAAAAAUg/iwt-_10hlFE/photo.jpg?sz=128"",""display_name"":""Vaibhav Vishal"",""link"":""https://stackoverflow.com/users/9321755/vaibhav-vishal""},""is_accepted"":true,""score"":0,""last_activity_date"":1538998730,""last_edit_date"":1538998730,""creation_date"":1538986184,""answer_id"":52697954,""question_id"":52696760}],""has_more"":true,""quota_max"":300,""quota_remaining"":298}");
            var client22 = mock22.Object;

            var result22 = client22.Answers.GetAll("stackoverflow.com", pagesize: 1, order: Order.Desc, sort: Answers.Sort.Activity, filter: "!9hnGsz84b").Result;

            Assert.True(result22.Success);

            var answer22 = result22.Data.Items.Single();

            Assert.NotNull(answer22.Tags);
            Assert.Equal(3, answer22.Tags.Length);
        }

        [Fact]
        public void Merge_get_test()
        {
            var mock = new Mock<StacManClient>(null, "2.2");

            //http://api.stackexchange.com/2.2/users/1450259/merges
            mock.FakeGET(response: @"{""items"":[{""merge_date"":1371139987,""new_account_id"":1450259,""old_account_id"":2885329}],""has_more"":false,""quota_max"":300,""quota_remaining"":284}");

            var client = mock.Object;

            var result = client.Users.GetMerges(new int[] { 1450259 }).Result;
            Assert.True(result.Success);

            var merge = result.Data.Items.Single();
            Assert.Equal(2885329, merge.OldAccountId);
            Assert.Equal(1450259, merge.NewAccountId);
            Assert.Equal(1371139987L.ToDateTime(), merge.MergeDate);
        }

        /// <summary>
        /// A vectorized version of this method was introduced in 2.1:
        /// http://api.stackexchange.com/docs/change-log
        /// </summary>
        [Fact]
        public void User_top_answer_tags_vectorized_test()
        {
            var mock = new Mock<StacManClient>(null, "2.2");

            //http://api.stackexchange.com/2.2/users/1/top-answer-tags?pagesize=3&site=stackoverflow
            mock.FakeGETForUrlPattern("/1/", response: @"{""items"":[{""user_id"":1,""answer_count"":19,""answer_score"":649,""question_count"":1,""question_score"":92,""tag_name"":""c#""},{""user_id"":1,""answer_count"":15,""answer_score"":494,""question_count"":1,""question_score"":-3,""tag_name"":""regex""},{""user_id"":1,""answer_count"":2,""answer_score"":371,""question_count"":0,""question_score"":0,""tag_name"":""arrays""}],""has_more"":true,""quota_max"":300,""quota_remaining"":291}");
            //http://api.stackexchange.com/2.2/users/1%3B3/top-answer-tags?pagesize=3&site=stackoverflow.com
            mock.FakeGETForUrlPattern("stackoverflow\\.com", response: @"{""items"":[{""user_id"":1,""answer_count"":19,""answer_score"":649,""question_count"":1,""question_score"":92,""tag_name"":""c#""},{""user_id"":3,""answer_count"":3,""answer_score"":625,""question_count"":0,""question_score"":0,""tag_name"":""html""},{""user_id"":3,""answer_count"":2,""answer_score"":618,""question_count"":0,""question_score"":0,""tag_name"":""jquery""}],""has_more"":true,""quota_max"":300,""quota_remaining"":290}");

            var client = mock.Object;

            var result = client.Users.GetTopAnswerTags("stackoverflow", 1, pagesize: 3).Result;
            var resultVectorized = client.Users.GetTopAnswerTags("stackoverflow.com", new int[] { 1, 3 }, pagesize: 3).Result;

            Assert.True(result.Success);
            Assert.True(resultVectorized.Success);

            Assert.Single(result.Data.Items.Select(i => i.UserId).Distinct());
            Assert.Equal(2, resultVectorized.Data.Items.Select(i => i.UserId).Distinct().Count());
        }

        // TODO find API which has been added in v2.2
        [Fact]
        public void Api_version_mismatch_test()
        {
            var mock = new Mock<StacManClient>(null, "2.1");

            var client = mock.Object;

            Assert2.Throws<InvalidOperationException>(() => client.Users.GetTopAnswerTags("stackoverflow.com", new int[] { 1, 3 }, pagesize: 3));
        }

        [Fact]
        public void Notice_test()
        {
            var mock = new Mock<StacManClient>(null, "2.2");

            //http://api.stackexchange.com/2.2/questions/1732348?order=desc&sort=activity&site=stackoverflow&filter=!9hnGsqOrt
            mock.FakeGET(response: @"{""items"":[{""tags"":[""html"",""regex"",""xhtml""],""notice"":{""owner_user_id"":102937,""creation_date"":1339098076,""body"":""<p>This post has been locked due to the high amount of off-topic comments generated. For extended discussions, please use <a href=\""https://chat.stackoverflow.com\"">chat</a>.</p>""},""owner"":{""reputation"":2612,""user_id"":142233,""user_type"":""registered"",""accept_rate"":100,""profile_image"":""https://i.stack.imgur.com/3h2RG.png?s=128&g=1"",""display_name"":""Jeff"",""link"":""https://stackoverflow.com/users/142233/jeff""},""is_answered"":true,""view_count"":2348892,""protected_date"":1291642187,""accepted_answer_id"":1732454,""answer_count"":35,""community_owned_date"":1258593236,""score"":1324,""locked_date"":1339098076,""last_activity_date"":1535647974,""creation_date"":1258151906,""last_edit_date"":1338064625,""question_id"":1732348,""link"":""https://stackoverflow.com/questions/1732348/regex-match-open-tags-except-xhtml-self-contained-tags"",""title"":""RegEx match open tags except XHTML self-contained tags""}],""has_more"":false,""quota_max"":300,""quota_remaining"":285}");

            var client = mock.Object;

            var result = client.Questions.GetByIds("stackoverflow", new int[] { 1732348 }, order: Order.Desc, sort: Questions.Sort.Activity, filter: "!9hnGsqOrt").Result;
            Assert.True(result.Success);

            var question = result.Data.Items.Single();
            Assert.NotNull(question.Notice);
            Assert.Equal("<p>This post has been locked due to the high amount of off-topic comments generated. For extended discussions, please use <a href=\"https://chat.stackoverflow.com\">chat</a>.</p>", question.Notice.Body);
            Assert.Equal(1339098076L.ToDateTime(), question.Notice.CreationDate);
            Assert.Equal(102937, question.Notice.OwnerUserId);
        }

        [Fact]
        public void Reputation_history_test()
        {
            var mock = new Mock<StacManClient>(null, "2.2");

            //http://api.stackexchange.com/2.2/users/2749/reputation-history?pagesize=3&site=stackoverflow
            mock.FakeGET(response: @"{""items"":[{""reputation_history_type"":""post_upvoted"",""reputation_change"":5,""post_id"":1060870,""creation_date"":1538726809,""user_id"":2749},{""reputation_history_type"":""post_upvoted"",""reputation_change"":10,""post_id"":8619489,""creation_date"":1538064016,""user_id"":2749},{""reputation_history_type"":""post_upvoted"",""reputation_change"":5,""post_id"":1060870,""creation_date"":1537797086,""user_id"":2749}],""has_more"":true,""quota_max"":300,""quota_remaining"":288}");

            var client = mock.Object;

            var result = client.Users.GetReputationHistory("stackoverflow", new int[] { 2749 }, pagesize: 3).Result;
            Assert.True(result.Success);

            var second = result.Data.Items[1];
            Assert.Equal(2749, second.UserId);
            Assert.Equal(1538064016L.ToDateTime(), second.CreationDate);
            Assert.Equal(8619489, second.PostId);
            Assert.Equal(10, second.ReputationChange);
            Assert.Equal(ReputationHistories.ReputationHistoryType.PostUpvoted, second.ReputationHistoryType);
        }

        [Fact]
        public void Reputation_Get_test()
        {
            var mock = new Mock<StacManClient>(null, "2.2");

            //http://api.stackexchange.com/2.2/users/23354/reputation?pagesize=5&site=stackoverflow
            mock.FakeGET(response: @"{""items"":[{""on_date"":1539025118,""reputation_change"":1025,""vote_type"":""up_votes"",""post_type"":""answer"",""post_id"":266718,""user_id"":23354},{""on_date"":1539023137,""reputation_change"":70,""vote_type"":""up_votes"",""post_type"":""answer"",""post_id"":16903497,""user_id"":23354},{""on_date"":1539021578,""reputation_change"":340,""vote_type"":""up_votes"",""post_type"":""answer"",""post_id"":355977,""user_id"":23354},{""on_date"":1539018304,""reputation_change"":100,""vote_type"":""up_votes"",""post_type"":""answer"",""post_id"":3967595,""user_id"":23354},{""on_date"":1539018106,""reputation_change"":290,""vote_type"":""up_votes"",""post_type"":""answer"",""post_id"":10613631,""user_id"":23354}],""has_more"":true,""backoff"":10,""quota_max"":300,""quota_remaining"":299}");

            var client = mock.Object;

            var result = client.Users.GetReputation("stackoverflow", new[] { 23354 }, pagesize: 5).Result;
            Assert.True(result.Success);

            var items = result.Data.Items;
            Assert.Equal(5, items.Length);
        }

        // TODO review for 2.2
        [Fact]
        public void Comment_add_test()
        {
            var mock = new Mock<StacManClient>(null, "2.1");

            //https://api.stackexchange.com/2.2/posts/3122/comments/add
            mock.FakePOST(response: @"{""items"":[{""post_id"":4490791,""creation_date"":1371346039,""edited"":false,""owner"":{""user_id"":2749,""display_name"":""Emmett"",""reputation"":5451,""profile_image"":""http://www.gravatar.com/avatar/129bc58fc3f1e3853cdd3cefc75fe1a0?d=identicon&r=PG"",""link"":""http://stackoverflow.com/users/2749/emmett"",""accept_rate"":76}}],""quota_remaining"":9997,""quota_max"":10000,""has_more"":false}");

            var client = mock.Object;

            var result = client.Posts.AddComment("stackoverflow", "access_token_123", 4490791, "This is a comment that I'm adding via the API!", preview: true).Result;
            Assert.True(result.Success);

            var comment = result.Data.Items.Single();
            Assert.Equal(4490791, comment.PostId);
        }

        // TODO review for 2.2
        [Fact]
        public void Comment_delete_test()
        {
            var mock = new Mock<StacManClient>(null, "2.1");

            //https://api.stackexchange.com/2.1/comments/4721972/delete
            mock.FakePOST(response: @"{""items"":[],""quota_remaining"":9992,""quota_max"":10000,""has_more"":false}");

            var client = mock.Object;

            var result = client.Comments.Delete("stackoverflow", "access_token_123", 4721972).Result;
            Assert.True(result.Success);

            Assert.Empty(result.Data.Items);
            Assert.Equal(10000, result.Data.QuotaMax);
        }
    }
}