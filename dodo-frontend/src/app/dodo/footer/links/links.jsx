import React from 'react'
import { Title } from "app/components/footer"
import { Link } from 'react-router-dom'
import styles from './links.module.scss'


import links from './library'

const linksTitle = "Around XR"

export const Links = () =>
    <div className={styles.linksBox}>
        <Title title={linksTitle}/>
        <ul className={styles.links}>
            {links.map(link =>
                <li key={link.name}>
                    <Link to={link.route}>{link.name}</Link>
                </li>
            )}

        </ul>
    </div>
