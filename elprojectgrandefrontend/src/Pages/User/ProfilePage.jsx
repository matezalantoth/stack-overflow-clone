import {useEffect, useState} from "react";
import QuestionsElement from "./QuestionsElement.jsx";
import AnswersElement from "./AnswersElement.jsx";
import {useCookies} from 'react-cookie';
import {useNavigate} from 'react-router-dom';


export default function ProfilePage() {
    const [user, setUser] = useState(null)
    const [selectedTab, setSelectedTab] = useState(null)
    const navigate = useNavigate();
    const [cookies] = useCookies(['user'])


    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const res = await fetch(`/api/Users/GetBySessionToken`,
                    {
                        headers: {
                            'Authorization': "Bearer " + cookies.user
                        }

                    });
                const data = await res.json();
                setUser(data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchUsers();
    }, []);

    useEffect(() => {
        if (!cookies.user) {
            navigate('/login')
        }

    }, [cookies])

    return user ? (
            <div className="Users bg-gray-50 min-h-screen p-8">
                <div className="user bg-white p-8 rounded-xl shadow-lg max-w-md mx-auto flex flex-col items-center">

                    {/* Profile Info */}
                    <div className="w-full">
                        <div className="text-center mb-4">
                            <h1 className="text-2xl font-bold text-gray-800">{user.name}</h1>
                            <p className="text-lg text-gray-500">{user.userName}</p>
                        </div>

                        <div className="bg-gray-100 p-4 rounded-lg shadow-sm">
                            <div className="flex items-center justify-between border-b py-2">
                                <span className="text-gray-600 font-medium">Email:</span>
                                <span className="text-gray-900">{user.email}</span>
                            </div>
                            <div className="flex items-center justify-between border-b py-2">
                                <span className="text-gray-600 font-medium">Karma:</span>
                                <span className="text-gray-900">{user.karma}</span>
                            </div>
                        </div>

                        <button
                            className="mt-6 bg-blue-500 text-white py-2 px-6 rounded-lg shadow hover:bg-blue-600 transition duration-300 w-full"
                            onClick={() => navigate('/editPage')}
                        >
                            Edit Profile
                        </button>
                    </div>
                </div>

                {/* Tabs for Answers/Questions */}
                <div className="flex justify-center mt-12">
        <span
            className={
                "text-2xl border-2 rounded-l-full px-6 py-2 transition-colors hover:bg-gray-200" +
                (selectedTab === 'answer' ? " bg-gray-200 text-gray-800 font-semibold" : " text-gray-600")
            }
            onClick={() => setSelectedTab('answer')}
        >
            Answers
        </span>
                    <span
                        className={
                            "text-2xl border-2 rounded-r-full px-6 py-2 border-l-4 transition-colors hover:bg-gray-200" +
                            (selectedTab === 'question' ? " bg-gray-200 text-gray-800 font-semibold" : " text-gray-600")
                        }
                        onClick={() => setSelectedTab('question')}
                    >
            Questions
        </span>
                </div>

                {/* Questions or Answers */}
                <div className="flex justify-center mt-8">
                    {selectedTab ? (
                        selectedTab === 'question' ? (
                            <QuestionsElement questions={user.questions}/>
                        ) : (
                            <AnswersElement answers={user.answers}/>
                        )
                    ) : (
                        <></>
                    )}
                </div>
            </div>
        ) :
        <div>
            loading

        </div>
}