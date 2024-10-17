// eslint-disable-next-line no-unused-vars,react/prop-types
import React, {useEffect, useState} from "react";
import {useCookies} from "react-cookie";
import {useNavigate} from "react-router-dom";
import ResultInteractionComponent from "./ResultInteractionComponent.jsx";
import {toast} from "react-hot-toast";
import {CheckIfSessionExpired} from "../../CheckIfSessionExpired.jsx";
import CreateTag from "./CreateTag.jsx";

export default function AdminPage({setUserLoginCookies}) {
    const [cookies] = useCookies(['user']);
    const navigate = useNavigate();
    const [adminCheck, setAdminCheck] = useState(false);
    const [searching, setSearching] = useState(null);
    const [searchingBy, setSearchingBy] = useState(null);
    const [showSearchBar, setShowSearchBar] = useState(false);
    const [searchBar, setSearchBar] = useState(null);
    const [searchData, setSearchData] = useState([]);
    const [searchResults, setSearchResults] = useState([]);
    const showErrorToast = (message) => toast.error(message);
    CheckIfSessionExpired(setUserLoginCookies);
    const [isOpen, setIsOpen] = React.useState(false);


    useEffect(() => {
        const fetchCheckIfAdmin = async () => {
            const res = await fetch('/api/users/isuseradmin',
                {
                    headers: {
                        'Authorization': "Bearer " + cookies.user
                    }
                })
            const data = await res.json()
            if (!data) {
                navigate('/');
                return;
            }
            setAdminCheck(() => true);
        }
        fetchCheckIfAdmin();
    }, [])

    useEffect(() => {
        setSearchingBy(() => null);
    }, [searching]);

    useEffect(() => {
        if (searching && searchingBy) {
            setSearchData(() => []);
            setSearchResults(() => []);
            setShowSearchBar(() => true);
            return;
        }
        setShowSearchBar(false);
    }, [searchingBy]);


    useEffect(() => {
        let url = "/api/admin";
        if (searchBar && showSearchBar) {
            switch (searching) {
                case "Users":
                    url += "/users/searchByUserName/" + searchBar;
                    break;
                case "Questions":
                    url += "/questions";
                    switch (searchingBy) {
                        case "Title":
                            url += "/searchByTitle/" + searchBar;
                            break;
                        case "Content":
                            url += "/searchByContent/" + searchBar;
                            break;
                    }
                    break;
                case "Answers":
                    url += "/answers/searchByContent/" + searchBar;
                    break;
                case "Tags":
                    url += "/tags";
                    switch (searchingBy) {
                        case "Name":
                            url += "/searchByTagName/" + searchBar;
                            break;
                        case "Description":
                            url += "/searchByTagDescription/" + searchBar;
                            break;
                    }
                    break;
            }
        } else {
            setSearchData(() => []);
            setSearchResults(() => []);
        }

        if (url.length > "/api/admin".length) {
            console.log(url);
            const fetchUrl = async () => {

                const res = await fetch(url, {
                    headers: {
                        'Authorization': "Bearer " + cookies.user
                    }
                })
                const data = await res.json();
                if (data.message) {
                    showErrorToast(data.message);
                    return;
                }
                setSearchData(() => data);
            }
            fetchUrl();
        }
    }, [searchBar]);

    useEffect(() => {
        if (searchData.length > 0 && showSearchBar) {
            switch (searching) {
                case "Users":
                    setSearchResults(() => searchData.map(u => {
                        return {value: u, id: u}
                    }));
                    break;
                case "Questions":
                    switch (searchingBy) {
                        case "Title":
                            setSearchResults(() => searchData.map(q => {
                                return {value: q.title, id: q.id}
                            }));
                            break;
                        case "Content":
                            setSearchResults(() => searchData.map(q => {
                                return {value: q.content, id: q.id}
                            }));
                            break;
                    }
                    break;
                case "Answers":
                    setSearchResults(() => searchData.map(a => {
                        return {value: a.content, id: a.id}
                    }));
                    break;
                case "Tags":
                    switch (searchingBy) {
                        case "Name":
                            setSearchResults(() => searchData.map(t => {
                                return {value: t.tagName, id: t.id}
                            }));
                            break;
                        case "Description":
                            setSearchResults(() => searchData.map(t => {
                                return {value: t.description, id: t.id}
                            }));
                            break;
                    }
            }
        }
    }, [searchData]);

    const toggleOpen = () => {
        setIsOpen(!isOpen);
    };


    return adminCheck ? (

        <div className="m-auto block mx-auto ml-6 mt-4">
            <span>
                Search
                {' '}
                <select
                    className="border-2 rounded-md focus:ring-2 focus:ring-gray-200 focus:rounded-md"
                    onChange={
                        (event) =>
                            event.target.value === 'Item' ?
                                setSearching(null) :
                                setSearching(event.target.value)}>
                    <option>Item</option>
                    <option>Users</option>
                    <option>Questions</option>
                    <option>Answers</option>
                    <option>Tags</option>
                </select>
                {' '}
                By
                {' '}
                <select
                    className="border-2 rounded-md focus:ring-2 focus:ring-gray-200 focus:rounded-md"
                    onChange={
                        (event) =>
                            event.target.value === 'Index' ?
                                setSearchingBy(null) :
                                setSearchingBy(event.target.value)}>
                    <option selected={!searchingBy}>Index</option>
                    {
                        searching === 'Users' ?
                            (<>
                                <option>Username</option>
                            </>) : searching === 'Questions' ? <>
                                <option>Title</option>
                                <option>Content</option>
                            </> : searching === 'Answers' ? <>
                                <option>Content</option>
                            </> : searching === 'Tags' ? <>
                                <option>Name</option>
                                <option>Description</option>
                            </> : <></>}
                </select>
            </span>
            <br/>
            {showSearchBar ?
                <div className="rounded-md border-2 w-1/5 mt-2">
                    <input
                        className="w-full"
                        onChange={(event) => setSearchBar(() => event.target.value === "" ? null : event.target.value)}
                        placeholder='Search...'/>
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
                                            searchModel={searching} id={u.id} setSearchResults={setSearchResults}/>
                                    </li>
                                </>;
                            })}
                        </ul> : <></>}
                </div> : <></>}
            <div className="relative left-3/4">
                <button
                    className="mr-2 border-2 border-blue-600 h-10 text-blue-600 w-20 px-2 rounded text-sm"
                    onClick={toggleOpen}
                >
                    Create Tag
                </button>
                {isOpen && (
                    <CreateTag />
                )}
            </div>
        </div>) : <>404</>
}