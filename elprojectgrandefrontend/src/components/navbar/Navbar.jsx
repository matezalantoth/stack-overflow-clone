import {useEffect, useState} from "react";
import {useLocation, useNavigate} from "react-router-dom";
import {faSearch} from "@fortawesome/free-solid-svg-icons";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";

export default function Navbar (props) {
    const {cookies} = props;
    const navigate = useNavigate();
    const location = useLocation();
    const [question, setQuestion] = useState(null);
    console.log(cookies.user);

    /*useEffect(() => {
        const fetchQuestion = async () => {
            const response = await fetch(`htt5166p://localhost:/Question/${id}`);
            const data = await response.json();
            setQuestion(data);
        }

        fetchQuestion();

    }, [])
        */

    return (<div className='inline-flex w-full'>

        <button className='ml-2 mt-2 border-2 border-blue-600 h-10 text-white px-2 w-20 bg-blue-600 rounded text-sm'
                onClick={() => {
                    navigate('/')
                }}>
            Grande
        </button>
        {location.pathname === '/login' || location.pathname === '/signup' || location.pathname === '/profile' ? <></> :
            <div className="w-full lg:w-1/2 mt-2 mx-auto">
                <div className="relative w-full">
                    <input
                        type="text"
                        className="w-full py-2 pl-10 pr-4 text-gray-700 bg-white border border-gray-300 rounded-full focus:border-blue-500 focus:outline-none focus:ring"
                        placeholder="Search..."
                    />
                    <div className="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
                        <FontAwesomeIcon className="text-gray-400" icon={faSearch}/>
                    </div>
                </div>
            </div>}
        {location.pathname === '/login' || location.pathname === '/signup' || location.pathname === '/profile' ? <></> :
            <>
                <button
                    className='mr-2 mt-2 border-2 border-blue-600 h-10 text-blue-600 w-20 px-2 rounded text-sm'
                    onClick={() => {
                        navigate(cookies.user ?  '/profile' : '/login');
                    }}>{cookies.user ? "Profile" : "Login"}
                </button>
            </>
        }

    </div>)
}
