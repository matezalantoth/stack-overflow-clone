import QuestionsElement from "./QuestionsElement.jsx";
import AnswersElement from "./AnswersElement.jsx";
import {useNavigate} from "react-router-dom";
import {useState} from "react";

export default  function ProfileRenderer ({user})
{
    const navigate = useNavigate()
    const[selectedTab, setSelectedTab] = useState(null)
    return user ?  (
            <div className="Users">

            

                <div className="m-auto flex justify-center mt-12 w-auto ">
                    <span className={"text-2xl  border-2 rounded-l-3xl  px-4 transition hover:bg-gray-200" + (selectedTab==='answer' ? " bg-gray-200" : "")}
                          onClick={() => {
                              setSelectedTab('answer')
                          }}>Answers</span>  <span className={"text-2xl px-4 transition hover:bg-gray-200 border-2 rounded-r-3xl border-l-4"  + (selectedTab==='question' ? " bg-gray-200" : "")} onClick={() => {
                    setSelectedTab('question')
                }}>Questions</span>
                </div>
                <div className="flex justify-center">
                    {selectedTab ? selectedTab === 'question' ?
                        <QuestionsElement questions={user.questions}/> :
                        <AnswersElement answers={user.answers}/> : <></>}
                </div>

            </div>
        ) :
        <div>
            loading

        </div>
}