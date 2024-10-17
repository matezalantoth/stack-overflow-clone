import {useNavigate} from "react-router-dom";
import React, {useEffect, useState} from "react";
import {toast} from "react-hot-toast";
import {useCookies} from "react-cookie";
import ResultInteractionComponent from "../Admin/ResultInteractionComponent.jsx";

export default function AskQuestion() {
    const [cookies] = useCookies(['user']);
    const navigate = useNavigate();
    const [question, setQuestion] = useState({});
    const [searching, setSearching] = useState('Tags');
    const [submittable, setSubmittable] = useState(false);
    const showErrorToast = (message) => toast.error(message);
    const showSuccessToast = (message) => toast.success(message);
    const [searchResults, setSearchResults] = useState([]);
    const [searchBar, setSearchBar] = useState(null);
    const [searchData, setSearchData] = useState([]);

    useEffect(() => {
        if (
            question.title &&
            question.content
        ) {
            setSubmittable(true);
        }
    }, [question]);

    useEffect(() => {
        if (!cookies.user) {
            navigate('/login');
        }
    }, [cookies])

    const createQuestion = async () => {
        const res = await fetch('/api/Questions', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Authorization': "Bearer " + cookies.user
            },
            body: JSON.stringify({
                ...question,
                sessionToken: cookies.user,
                postedAt: new Date(Date.now()).toISOString()
            }),
        });
        return await res.json();
    }

    useEffect(() => {
        let url = "/api/Tags/search/"
            if(searchBar){
                url += searchBar;
            } else {
                setSearchData(() => []);
                setSearchResults(() => []);
            }

        const fetchUrl = async () => {
            const res = await fetch(url, {
                headers: {
                    'Authorization': "Bearer " + cookies.user
                }
            });
            const data = await res.json();
            setSearchData(() => data);
        }
        fetchUrl();
    }, [searchBar]);

    useEffect(() => {
        if (searchData.length > 0) {
            setSearchResults(() => searchData.map(t => {
                return {value: t.tagName, id: t.id}
            }));
        }
    }, [searchData]);


    return (
        <div className="flex flex-col items-center justify-center mt-24 p-4">
            <div className="w-full max-w-lg bg-white rounded-lg shadow-lg p-6">
                <h1 className="text-2xl font-semibold text-gray-900 mb-6 text-center">
                    Ask a Question
                </h1>
                <form className="space-y-6" action="#">
                    <div className="flex flex-col">
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Title
                        </label>
                        <textarea
                            onChange={(event) => {
                                setQuestion({...question, title: event.target.value});
                            }}
                            required
                            placeholder="Enter your question title"
                            className="border border-gray-300 rounded-lg px-4 py-2 resize-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        ></textarea>
                    </div>

                    <div className="flex flex-col">
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Details of your question
                        </label>
                        <textarea
                            onChange={(event) => {
                                setQuestion({...question, content: event.target.value});
                            }}
                            required
                            placeholder="Provide more details about your question"
                            className="border border-gray-300 rounded-lg px-4 py-2 h-32 resize-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        ></textarea>
                    </div>

                    <div className="flex flex-col">
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Please enter any tags, max 5
                        </label>
                        <input
                            onChange={(event) => setSearchBar(() => event.target.value === "" ? null : event.target.value)}
                            placeholder='Search...'
                            className="border border-gray-300 rounded-lg px-4 py-2 resize-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        ></input>
                        {searchResults.length > 0 ?
                            <ul>
                                {searchResults.map((u, i) => {
                                    return <>
                                        <li className="p-2 border-b-2 border-gray-200"
                                            key={i}>
                                            <div className="w-4/5 inline-block">
                                                <div className="truncate">{u.value}</div>

                                            </div>
                                            <ResultInteractionComponent
                                                searchModel={searching} id={u.id}/>
                                        </li>
                                    </>;
                                })}
                            </ul> : <></>}
                    </div>

                    <button
                        className="w-full bg-blue-500 hover:bg-blue-600 text-white font-semibold py-2 rounded-lg transition duration-200 shadow-md focus:ring-2 focus:ring-blue-300"
                        onClick={(event) => {
                            event.preventDefault();
                            createQuestion(event).then((data) => {
                                if (data.id) {
                                    showSuccessToast("Successfully posted question!");
                                    navigate('/question/' + data.id);
                                    return;
                                }
                                showErrorToast("Something went wrong")

                            });
                        }}
                    >
                        Submit
                    </button>
                </form>
            </div>
        </div>
    );
}