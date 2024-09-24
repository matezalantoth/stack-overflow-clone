import {useEffect, useState} from "react";
import {useLocation, useNavigate} from "react-router-dom";
import {faSearch} from "@fortawesome/free-solid-svg-icons";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {useCookies} from "react-cookie";

export default function Navbar({setUserLoginCookies, setsearchQuestion, normalQuestion}) {
    const navigate = useNavigate();
    const location = useLocation();
    const [cookies] = useCookies(['user']);
    const [cookiesHaveBeenSet, setCookiesHaveBeenSet] = useState(false);
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredQuestions, setFilteredQuestions] = useState([]);
    const [showDropdown, setShowDropdown] = useState(false);
    const [selectedQuestionId, setSelectedQuestionId] = useState(null);
    const handleLogout = async () => {
        await fetch("api/users/logout", {
            method: 'POST',
            headers: {
                'Authorization': "Bearer " + cookies.user
            }
        });
        setUserLoginCookies(null);
        setCookiesHaveBeenSet(true);
    };

    useEffect(() => {
        if (cookiesHaveBeenSet) {
            navigate('/login');
            setCookiesHaveBeenSet(false);
        }
    }, [cookies, cookiesHaveBeenSet]);

    useEffect(() => {
        if (searchTerm) {

            setFilteredQuestions(normalQuestion.filter((q) =>
                q.title.toLowerCase().includes(searchTerm.toLowerCase())));
            setShowDropdown(true);
        } else {
            setFilteredQuestions([]);
            setShowDropdown(false);
        }
    }, [searchTerm, normalQuestion]);

    useEffect(() => {
        if (selectedQuestionId && !showDropdown) {
            setSelectedQuestionId(() => null)
            setSearchTerm(() => null);
            navigate(`/question/${selectedQuestionId}`);
            console.log(selectedQuestionId)
            console.log(showDropdown)
        }
    }, [selectedQuestionId, showDropdown]);


    return (
        <div className="flex w-full justify-between items-center px-4 py-2">
            <div className="flex items-center">
                <button
                    className="ml-2 border-2 border-blue-600 h-10 text-white px-2 w-20 bg-blue-600 rounded text-sm"
                    onClick={() => navigate('/')}
                >
                    Grande
                </button>
                {location.pathname === '/' && (
                    <button
                        onClick={() => navigate('/askquestion')}
                        className="ml-2 border-2 border-amber-400 h-10 text-white px-2 w-20 bg-amber-400 rounded text-sm"
                    >
                        Ask
                    </button>
                )}
            </div>

            {location.pathname === '/' && (
                <div className="w-full lg:w-1/2 mx-auto relative">
                    <div className="relative w-full">
                        <input
                            type="text"
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="w-full py-2 pl-10 pr-4 text-gray-700 bg-white border border-gray-300 rounded-full focus:border-blue-500 focus:outline-none focus:ring"
                            placeholder="Search..."
                        />
                        <div className="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
                            <FontAwesomeIcon className="text-gray-400" icon={faSearch}/>
                        </div>
                    </div>
                    {showDropdown && filteredQuestions.length > 0 && normalQuestion ? (
                        <ul className="absolute z-10 w-full bg-white border border-gray-300 rounded-lg mt-1 max-h-60 overflow-y-auto shadow-lg">
                            {filteredQuestions.map((question, index) => (
                                <li
                                    key={index}
                                    onClick={() => {
                                        setShowDropdown(() => false);
                                        setSearchTerm(() => question.title);
                                        setSelectedQuestionId(() => question.id)
                                    }}
                                    className="px-4 py-2 hover:bg-blue-100 cursor-pointer text-gray-700"
                                >
                                    {question.title}
                                </li>
                            ))}
                        </ul>
                    ) : <></>}
                </div>
            )}

            <div className="flex items-center">
                {!cookies.user && location.pathname !== '/login' && location.pathname !== '/signup' && (
                    <button
                        className="mr-2 border-2 border-black h-10 text-black-700 w-20 px-2 rounded text-sm hover:underline"
                        onClick={() => navigate('/signup')}
                    >
                        Sign up
                    </button>
                )}
                {location.pathname !== '/login' && location.pathname !== '/signup' && (
                    <button
                        className="mr-2 border-2 border-blue-600 h-10 text-blue-600 w-20 px-2 rounded text-sm"
                        onClick={() => navigate(cookies.user ? '/profile' : '/login')}
                    >
                        {cookies.user ? "Profile" : "Login"}
                    </button>
                )}
                {cookies.user && (
                    <button
                        className="mr-2 border-2 border-red-600 h-10 text-red-700 w-20 px-2 rounded text-sm hover:underline"
                        onClick={handleLogout}
                    >
                        Logout
                    </button>
                )}
            </div>
        </div>
    )
}
