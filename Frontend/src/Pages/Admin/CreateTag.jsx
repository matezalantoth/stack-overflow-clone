import React from 'react';
import {useCookies} from "react-cookie";
import {toast} from "react-hot-toast";

export default function CreateTag() {
    const [cookies] = useCookies(['user']);
    const [tagData, setTagData] = React.useState({
        tagName: null,
        description: null
    });

    const showErrorToast = (message) => toast.error(message);
    const showSuccessToast = (message) => toast.success(message);

    const createTag = async () => {
        const response = await fetch('/api/Tags', {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + cookies.user
            },
            body: JSON.stringify(tagData),
        });
        return await response.json();
    }


    return (
        <div className="relative w-72">
            <form className='space-y-6' action='#'>
                {/*<h5 className='text-xl font-medium text-gray-900'>*/}
                {/*    Make a Tag!*/}
                {/*</h5>*/}
                <div>
                    <label className='block mb-2 mt-2 text-sm font-medium text-gray-900'>
                        Tag Name
                    </label>
                    <input
                        onChange={(event) => {
                            setTagData({...tagData, tagName: event.target.value});
                        }}
                        type='text'
                        className='bg-gray-50 border border-gray-300  text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 '
                        placeholder='tagname'
                        required
                    />
                </div>
                <div>
                    <label className='block mb-2 text-sm font-medium text-gray-900 '>
                        Tag Description
                    </label>
                    <input
                        onChange={(event) => {
                            setTagData({
                                ...tagData,
                                description: event.target.value,
                            });
                        }}
                        type='text'
                        placeholder='description'
                        className='bg-gray-50 border border-gray-300  text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 '
                        required
                    />
                </div>
                <button
                    onClick={async (event) => {
                        event.preventDefault();
                        {
                            try {
                                const data = await createTag();
                                console.log(data);
                                showSuccessToast('Tag Created Successfully!');

                            } catch (e) {
                                console.error(e);
                                showErrorToast("Something went wrong");
                            }
                        }
                    }}
                    type='submit'
                    className='w-full  bg-blue-500 hover:bg-blue-600 focus:ring-4 focus:outline-nonefont-medium rounded-lg text-sm px-5 py-2.5 text-center'
                >
                    Create tag
                </button>
            </form>
        </div>
    );
}

