/* eslint-disable react/prop-types */
import {fetchAnswers, postAnswer} from "./QuestionPageServiceProvider.jsx";
import {toast} from "react-hot-toast";

export const MessageComponent = ({questionId, reply, submittable, cookies, setReply, setAnswers, setSubmittable}) => {
    const showErrorToast = (message) => toast.error(message);
    const showSuccessToast = (message) => toast.success(message);
    return (<div className="w-3/4 mx-auto p-4 bg-white shadow-lg rounded-lg mt-2">
        <form>
            <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2">Your Reply</label>
                <textarea
                    onChange={(event) => {
                        setReply({...reply, content: event.target.value});
                    }}
                    id="reply"
                    rows="5"
                    className="resize-none w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
                    placeholder="Write your reply here..."
                ></textarea>
            </div>

            <div className="flex justify-end">
                <button
                    onClick={async (event) => {
                        event.preventDefault();
                        const data = await postAnswer(questionId, cookies, reply, showErrorToast);
                        if (!data.content) {
                            showErrorToast("Something went wrong");
                            return;
                        }
                        showSuccessToast("Successfully posted answer!");
                        fetchAnswers(questionId, setAnswers);
                        setReply({content: ""});
                        setSubmittable(false);
                    }}
                    disabled={!submittable}
                    type="submit"
                    className="disabled:bg-gray-400 bg-blue-500 text-white font-bold py-2 px-4 rounded-lg hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-400 focus:ring-opacity-50"
                >
                    Submit
                </button>
            </div>
        </form>
    </div>)
}